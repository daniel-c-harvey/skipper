import psycopg
import random
from datetime import datetime, timedelta
from typing import Dict, List, Tuple

# Database connection configuration (used only for querying existing data)
# For WSL: Use Windows host IP instead of localhost
DB_CONFIG = {
    'host': 'localhost',
    'dbname': 'mauto-test',
    'user': 'mauto_agent',
    'password': 'midwest!best',
    'port': 5432
}

# Rental agreement constants - Updated to match enum values
RENTAL_STATUSES = ['Quoted', 'Pending', 'Active', 'Expired', 'Cancelled']
PRICE_UNITS = ['PerDay', 'PerWeek', 'PerMonth', 'PerYear']
TARGET_RECORDS = 100000

class RentalAgreementGenerator:
    def __init__(self, db_config: Dict):
        self.db_config = db_config
        self.conn = None
        self.slips = []
        self.vessels = []
        self.slip_classifications = {}
        
    def connect(self):
        """Establish database connection"""
        try:
            self.conn = psycopg.connect(**self.db_config)
            print("✓ Database connection established")
        except Exception as e:
            print(f"✗ Database connection failed: {e}")
            raise
    
    def fetch_data(self):
        """Fetch slips, vessels, and slip classifications"""
        cursor = self.conn.cursor()
        
        # Fetch slip classifications
        cursor.execute("""
            SELECT "Id", "MaxLength", "MaxBeam", "BasePrice" 
            FROM skipper.slip_classifications 
            WHERE "IsDeleted" = false
        """)
        for row in cursor.fetchall():
            self.slip_classifications[row[0]] = {
                'max_length': float(row[1]),
                'max_beam': float(row[2]), 
                'base_price': int(row[3])
            }
        
        # Fetch available slips
        cursor.execute("""
            SELECT s."Id", s."SlipClassificationId", s."Status" 
            FROM skipper.slips s 
            WHERE s."IsDeleted" = false 
            AND s."Status" IN ('Available', 'Booked')
        """)
        self.slips = cursor.fetchall()
        
        # Fetch vessels
        cursor.execute("""
            SELECT "Id", "Length", "Beam", "VesselType" 
            FROM skipper.vessels 
            WHERE "IsDeleted" = false
        """)
        self.vessels = cursor.fetchall()
        
        cursor.close()
        print(f"✓ Fetched {len(self.slips)} slips, {len(self.vessels)} vessels, {len(self.slip_classifications)} classifications")
    
    def can_vessel_fit_slip(self, vessel: Tuple, slip_id: int, classification_id: int) -> bool:
        """Check if vessel can fit in slip based on dimensions"""
        if classification_id not in self.slip_classifications:
            return False
            
        classification = self.slip_classifications[classification_id]
        vessel_length = float(vessel[1])
        vessel_beam = float(vessel[2])
        
        return (vessel_length <= classification['max_length'] and 
                vessel_beam <= classification['max_beam'])
    
    def generate_date_range(self) -> Tuple[datetime, datetime]:
        """Generate realistic rental date range"""
        # Random start date in the last 2 years or next 6 months
        start_base = datetime(2022, 1, 1)
        end_base = datetime(2025, 6, 30)
        days_range = (end_base - start_base).days
        
        start_date = start_base + timedelta(days=random.randint(0, days_range))
        
        # Rental duration: 1-30 days (weighted toward shorter stays)
        duration_weights = [0.3, 0.25, 0.2, 0.15, 0.1]  # 1-7, 8-14, 15-21, 22-30, 30+ days
        duration_ranges = [(1, 7), (8, 14), (15, 21), (22, 30), (31, 90)]
        duration_range = random.choices(duration_ranges, weights=duration_weights)[0]
        duration = random.randint(*duration_range)
        
        end_date = start_date + timedelta(days=duration)
        return start_date, end_date
    
    def generate_price_rate(self, classification_id: int) -> Tuple[int, str]:
        """Generate price rate based on slip classification and price unit"""
        if classification_id not in self.slip_classifications:
            return 5000, 'PerDay'  # Default fallback
            
        base_price = self.slip_classifications[classification_id]['base_price']
        price_unit = random.choices(PRICE_UNITS, weights=[0.5, 0.3, 0.15, 0.05])[0]
        
        # Apply pricing multipliers based on unit
        multiplier = 1.0
        if price_unit == 'PerWeek':
            multiplier = 6.5  # Weekly discount (~7% off daily rate)
        elif price_unit == 'PerMonth':
            multiplier = 25.0  # Monthly discount (~17% off daily rate)
        elif price_unit == 'PerYear':
            multiplier = 280.0  # Annual discount (~23% off daily rate)
            
        # Add random variation (±20%)
        variation = random.uniform(0.8, 1.2)
        price_rate = int(base_price * multiplier * variation)
        
        return price_rate, price_unit
    
    def generate_status(self, start_date: datetime, end_date: datetime) -> str:
        """Generate realistic status based on rental dates"""
        now = datetime.now()
        
        if end_date < now:
            # Past rentals: mostly expired, some cancelled
            return random.choices(['Expired', 'Cancelled'], weights=[0.85, 0.15])[0]
        elif start_date <= now <= end_date:
            # Current rentals: active
            return 'Active'
        else:
            # Future rentals: quoted or pending
            return random.choices(['Quoted', 'Pending'], weights=[0.4, 0.6])[0]
    
    def find_compatible_matches(self) -> List[Tuple]:
        """Find all compatible vessel-slip matches based on dimensions"""
        matches = []
        
        for vessel in self.vessels:
            vessel_id = vessel[0]
            compatible_slips = []
            
            for slip in self.slips:
                slip_id, classification_id, slip_status = slip
                if self.can_vessel_fit_slip(vessel, slip_id, classification_id):
                    compatible_slips.append(slip)
            
            # Add this vessel with all its compatible slips
            if compatible_slips:
                matches.extend([(vessel_id, slip[0], slip[1]) for slip in compatible_slips])
        
        return matches
    
    def generate_rental_agreements(self) -> List[Dict]:
        """Generate rental agreement records"""
        print("Finding compatible vessel-slip matches...")
        matches = self.find_compatible_matches()
        
        if not matches:
            raise Exception("No compatible vessel-slip matches found")
            
        print(f"✓ Found {len(matches)} compatible vessel-slip combinations")
        
        # Generate records
        records = []
        random.seed(42)  # For reproducible results
        
        for i in range(TARGET_RECORDS):
            vessel_id, slip_id, classification_id = random.choice(matches)
            
            start_date, end_date = self.generate_date_range()
            price_rate, price_unit = self.generate_price_rate(classification_id)
            status = self.generate_status(start_date, end_date)
            
            # Use start_date as creation timestamp with some variation
            created_at = start_date - timedelta(days=random.randint(1, 30))
            updated_at = created_at + timedelta(hours=random.randint(0, 24))
            
            record = {
                'slip_id': slip_id,
                'vessel_id': vessel_id,
                'start_date': start_date,
                'end_date': end_date,
                'price_rate': price_rate,
                'price_unit': price_unit,
                'status': status,
                'created_at': created_at,
                'updated_at': updated_at
            }
            records.append(record)
            
            if (i + 1) % 5000 == 0:
                print(f"Generated {i + 1}/{TARGET_RECORDS} records...")
        
        return records
    
    def write_sql_files(self, records: List[Dict]):
        """Write rental agreement records to batched SQL files"""
        batch_size = 5000
        total_batches = (len(records) - 1) // batch_size + 1
        
        for batch_num in range(total_batches):
            start_idx = batch_num * batch_size
            end_idx = min(start_idx + batch_size, len(records))
            batch = records[start_idx:end_idx]
            
            filename = f'rental_agreements_batch_{batch_num + 1:02d}.sql'
            
            with open(filename, 'w') as f:
                # Write header
                f.write('-- Rental Agreement Data Generation Script\n')
                f.write(f'-- Batch {batch_num + 1} of {total_batches}\n')
                f.write(f'-- Records {start_idx + 1}-{end_idx} of {len(records)} total records\n')
                f.write('-- Generated from compatible vessel-slip matches based on dimensional constraints\n')
                f.write('-- Uses RentalStatus: Quoted, Pending, Active, Expired, Cancelled\n')
                f.write('-- Uses PriceUnit: PerDay, PerWeek, PerMonth, PerYear\n')
                f.write('-- Realistic rental periods, pricing, and status distribution\n\n')
                f.write('BEGIN;\n\n')
                
                # Write INSERT statement
                f.write('INSERT INTO skipper.rental_agreements ("SlipId", "VesselId", "StartDate", "EndDate", "PriceRate", "PriceUnit", "Status", "CreatedAt", "UpdatedAt") VALUES\n')
                
                # Generate SQL values
                sql_values = []
                for record in batch:
                    start_date_str = record['start_date'].strftime('%Y-%m-%d %H:%M:%S+00')
                    end_date_str = record['end_date'].strftime('%Y-%m-%d %H:%M:%S+00')
                    created_at_str = record['created_at'].strftime('%Y-%m-%d %H:%M:%S+00')
                    updated_at_str = record['updated_at'].strftime('%Y-%m-%d %H:%M:%S+00')
                    
                    # Escape single quotes in status and price_unit
                    status_escaped = record['status'].replace("'", "''")
                    price_unit_escaped = record['price_unit'].replace("'", "''")
                    
                    value = f"({record['slip_id']}, {record['vessel_id']}, '{start_date_str}', '{end_date_str}', {record['price_rate']}, '{price_unit_escaped}', '{status_escaped}', '{created_at_str}', '{updated_at_str}')"
                    sql_values.append(value)
                
                f.write(',\n'.join(sql_values))
                f.write(';\n\n')
                f.write('COMMIT;\n\n')
                
                # Write batch summary
                batch_status_counts = {}
                batch_unit_counts = {}
                for record in batch:
                    batch_status_counts[record['status']] = batch_status_counts.get(record['status'], 0) + 1
                    batch_unit_counts[record['price_unit']] = batch_unit_counts.get(record['price_unit'], 0) + 1
                
                f.write('-- Batch Summary:\n')
                f.write(f'-- Records in batch: {len(batch)}\n')
                f.write(f'-- Status distribution: {dict(batch_status_counts)}\n')
                f.write(f'-- Price unit distribution: {dict(batch_unit_counts)}\n')
                f.write('-- RentalStatus values: Quoted, Pending, Active, Expired, Cancelled\n')
                f.write('-- PriceUnit values: PerDay, PerWeek, PerMonth, PerYear\n')
                f.write('-- All records reference existing SlipId and VesselId\n')
                f.write('-- Rental periods are realistic with proper date ranges\n')
                f.write('-- Pricing is based on slip classification with unit-based multipliers\n')
            
            print(f"✓ Generated {filename} with {len(batch)} records")
        
        print(f"✓ Successfully generated {total_batches} SQL files with {len(records)} total rental agreements")
    
    def generate_and_write_sql(self):
        """Main method to generate rental agreements and write SQL files"""
        try:
            self.connect()
            self.fetch_data()
            records = self.generate_rental_agreements()
            self.write_sql_files(records)
            
            # Print summary statistics
            status_counts = {}
            unit_counts = {}
            for record in records:
                status_counts[record['status']] = status_counts.get(record['status'], 0) + 1
                unit_counts[record['price_unit']] = unit_counts.get(record['price_unit'], 0) + 1
            
            print("\n📊 Generation Summary:")
            print(f"Total records: {len(records)}")
            print(f"Status distribution: {status_counts}")
            print(f"Price unit distribution: {unit_counts}")
            print(f"RentalStatus enum: {RENTAL_STATUSES}")
            print(f"PriceUnit enum: {PRICE_UNITS}")
            
        except Exception as e:
            print(f"✗ Error: {e}")
        finally:
            if self.conn:
                self.conn.close()
                print("✓ Database connection closed")

def main():
    """Main entry point"""
    print("🚢 Skipper Rental Agreement SQL Generator")
    print("=" * 50)
    
    generator = RentalAgreementGenerator(DB_CONFIG)
    generator.generate_and_write_sql()

if __name__ == "__main__":
    main() 