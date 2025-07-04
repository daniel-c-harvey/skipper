import random
from datetime import datetime, timedelta

# SlipClassification IDs from the database (51-88)
slip_classification_ids = list(range(1, 38))

# Marina location codes (different docks/piers)
location_codes = [
    'DOCK-A', 'DOCK-B', 'DOCK-C', 'DOCK-D', 'DOCK-E', 'DOCK-F',
    'PIER-1', 'PIER-2', 'PIER-3', 'PIER-4', 'PIER-5', 'PIER-6',
    'NORTH', 'SOUTH', 'EAST', 'WEST', 'CENTER', 'MARINA-1', 'MARINA-2'
]

# SlipStatus string values (from SkipperModels)
slip_statuses = ['Available', 'Booked', 'InUse', 'Maintenance', 'Sold', 'Archived']

def generate_timestamp():
    """Generate a random timestamp between 2020 and 2024"""
    start_date = datetime(2020, 1, 1)
    end_date = datetime(2024, 12, 31)
    time_between = end_date - start_date
    days_between = time_between.days
    random_days = random.randrange(days_between)
    random_seconds = random.randrange(86400)
    return start_date + timedelta(days=random_days, seconds=random_seconds)

def generate_slip_number(location_code, slip_num):
    """Generate a realistic slip number based on location and number"""
    if location_code.startswith('DOCK'):
        return f"{location_code}-{slip_num:03d}"
    elif location_code.startswith('PIER'):
        return f"{location_code}-{slip_num:02d}"
    else:
        return f"{location_code}-{slip_num:03d}"

def get_weighted_status():
    """Return a status with realistic distribution"""
    weights = [60, 25, 8, 4, 2, 1]  # Available, Booked, InUse, Maintenance, Sold, Archived
    return random.choices(slip_statuses, weights=weights)[0]

# Set seed for reproducible results
random.seed(42)

# Generate 5-8 slips per classification (total ~250 slips)
slips_per_classification = random.choices([5, 6, 7, 8], weights=[20, 40, 30, 10], k=len(slip_classification_ids))
total_slips = sum(slips_per_classification)

print(f'Generating Slip SQL file with {total_slips} records...')

with open('slips.sql', 'w') as f:
    f.write('-- Slip Data Generation Script\n')
    f.write(f'-- Generates {total_slips} slip records\n')
    f.write('-- References SlipClassification IDs 51-88\n')
    f.write('-- Realistic slip numbers, location codes, and status distribution\n')
    f.write('-- Status: Available, Booked, InUse, Maintenance, Sold, Archived\n\n')
    f.write('BEGIN;\n\n')
    
    f.write('INSERT INTO skipper.slips ("SlipClassificationId", "SlipNumber", "LocationCode", "Status", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES\n')
    
    records = []
    used_slip_numbers = set()
    
    for i, classification_id in enumerate(slip_classification_ids):
        num_slips = slips_per_classification[i]
        location_code = random.choice(location_codes)
        
        for j in range(num_slips):
            # Ensure unique slip numbers
            attempts = 0
            while True:
                slip_num = random.randint(1, 999)
                slip_number = generate_slip_number(location_code, slip_num)
                if slip_number not in used_slip_numbers:
                    used_slip_numbers.add(slip_number)
                    break
                attempts += 1
                if attempts > 100:
                    slip_number = f"{location_code}-{len(used_slip_numbers) + 1:03d}"
                    used_slip_numbers.add(slip_number)
                    break
            
            status = get_weighted_status()
            timestamp = generate_timestamp()
            timestamp_str = timestamp.strftime('%Y-%m-%d %H:%M:%S+00')
            
            record = f"({classification_id}, '{slip_number}', '{location_code}', '{status}', '{timestamp_str}', '{timestamp_str}', false)"
            records.append(record)
    
    f.write(',\n'.join(records))
    f.write(';\n\n')
    
    f.write('COMMIT;\n\n')
    f.write('-- Script Summary:\n')
    f.write(f'-- Total Records: {total_slips}\n')
    f.write('-- SlipClassification References: 51-88 (38 classifications)\n')
    f.write('-- Location Codes: 19 different marina areas\n')
    f.write('-- Status Distribution: ~60% Available, ~25% Booked, ~8% InUse, ~4% Maintenance, ~2% Sold, ~1% Archived\n')
    f.write('-- All slip numbers are unique\n')
    f.write('-- Timestamps distributed between 2020-2024\n')
    f.write('-- All records have IsDeleted = false\n')

print(f'Slip SQL file generated: slips.sql')
print(f'File contains {total_slips} slip records across {len(slip_classification_ids)} classifications')
print('Status distribution follows realistic marina patterns') 
