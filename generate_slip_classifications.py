import random
from datetime import datetime, timedelta

# Define slip classification categories with unique dimensions
slip_categories = [
    # Small Vessels
    {"name": "Compact Runabout", "max_length": 18.0, "max_beam": 7.0, "base_price": 2500},
    {"name": "Small Bowrider", "max_length": 20.0, "max_beam": 7.5, "base_price": 2800},
    {"name": "Small Cuddy Cabin", "max_length": 22.0, "max_beam": 8.0, "base_price": 3200},
    {"name": "Small Cruiser", "max_length": 24.0, "max_beam": 8.5, "base_price": 3600},
    {"name": "Small Sport Boat", "max_length": 25.0, "max_beam": 8.5, "base_price": 3800},
    
    # Medium Vessels
    {"name": "Medium Bowrider", "max_length": 26.0, "max_beam": 9.0, "base_price": 4200},
    {"name": "Medium Express", "max_length": 28.0, "max_beam": 9.5, "base_price": 4800},
    {"name": "Medium Sport Yacht", "max_length": 30.0, "max_beam": 10.0, "base_price": 5600},
    
    # Large Vessels
    {"name": "Large Express Cruiser", "max_length": 33.0, "max_beam": 10.5, "base_price": 6800},
    {"name": "Large Sport Yacht", "max_length": 34.0, "max_beam": 11.0, "base_price": 7200},
    {"name": "Large Cabin Cruiser", "max_length": 35.0, "max_beam": 11.0, "base_price": 7600},
    
    # Extra Large Vessels
    {"name": "Extra Large Express", "max_length": 36.0, "max_beam": 11.5, "base_price": 8200},
    {"name": "Extra Large Sport Yacht", "max_length": 38.0, "max_beam": 12.0, "base_price": 9200},
    {"name": "Extra Large Cabin Yacht", "max_length": 40.0, "max_beam": 12.5, "base_price": 10200},
    
    # Premium Vessels
    {"name": "Premium Motor Yacht", "max_length": 42.0, "max_beam": 13.0, "base_price": 11400},
    {"name": "Premium Sport Yacht", "max_length": 44.0, "max_beam": 13.5, "base_price": 12600},
    
    # Luxury Vessels
    {"name": "Luxury Express Yacht", "max_length": 46.0, "max_beam": 14.0, "base_price": 14000},
    {"name": "Luxury Cabin Yacht", "max_length": 48.0, "max_beam": 14.5, "base_price": 15400},
    {"name": "Luxury Mega Yacht", "max_length": 50.0, "max_beam": 15.0, "base_price": 16800},
    
    # Super Luxury Vessels
    {"name": "Super Luxury Express", "max_length": 52.0, "max_beam": 15.0, "base_price": 18200},
    {"name": "Super Luxury Motor Yacht", "max_length": 54.0, "max_beam": 15.5, "base_price": 19600},
    {"name": "Super Luxury Cabin Yacht", "max_length": 56.0, "max_beam": 16.0, "base_price": 21200},
    {"name": "Super Luxury Mega Yacht", "max_length": 60.0, "max_beam": 16.5, "base_price": 24200},
    
    # Mega Yacht Category
    {"name": "Mega Yacht Standard", "max_length": 65.0, "max_beam": 17.0, "base_price": 27000},
    {"name": "Mega Yacht Deluxe", "max_length": 70.0, "max_beam": 17.5, "base_price": 30200},
    {"name": "Mega Yacht Premium", "max_length": 75.0, "max_beam": 18.0, "base_price": 33600},
    
    # Super Yacht Category
    {"name": "Super Yacht Standard", "max_length": 80.0, "max_beam": 18.5, "base_price": 37500},
    {"name": "Super Yacht Deluxe", "max_length": 85.0, "max_beam": 19.0, "base_price": 41800},
    {"name": "Super Yacht Premium", "max_length": 90.0, "max_beam": 19.5, "base_price": 46500},
    {"name": "Super Yacht Elite", "max_length": 95.0, "max_beam": 20.0, "base_price": 51600},
    {"name": "Super Yacht Ultimate", "max_length": 100.0, "max_beam": 20.5, "base_price": 57200},
    
    # Ultra Luxury Category
    {"name": "Ultra Luxury Standard", "max_length": 110.0, "max_beam": 21.0, "base_price": 64500},
    {"name": "Ultra Luxury Deluxe", "max_length": 120.0, "max_beam": 22.0, "base_price": 72800},
    {"name": "Ultra Luxury Premium", "max_length": 130.0, "max_beam": 23.0, "base_price": 82200},
    {"name": "Ultra Luxury Elite", "max_length": 140.0, "max_beam": 24.0, "base_price": 92800},
    {"name": "Ultra Luxury Ultimate", "max_length": 150.0, "max_beam": 25.0, "base_price": 104500},
    
    # Commercial/Charter Category
    {"name": "Charter Vessel Standard", "max_length": 80.0, "max_beam": 18.0, "base_price": 35000},
    {"name": "Charter Vessel Large", "max_length": 100.0, "max_beam": 20.0, "base_price": 55000}
]

def remove_duplicates(categories):
    """Remove duplicate slip classifications based on unique max_length/max_beam combinations"""
    seen = set()
    unique_categories = []
    
    for category in categories:
        key = (category["max_length"], category["max_beam"])
        if key not in seen:
            seen.add(key)
            unique_categories.append(category)
    
    return unique_categories

def generate_timestamp():
    """Generate a random timestamp between 2020 and 2024 for slip classification creation dates"""
    start_date = datetime(2020, 1, 1)
    end_date = datetime(2024, 12, 31)
    time_between = end_date - start_date
    days_between = time_between.days
    random_days = random.randrange(days_between)
    random_seconds = random.randrange(86400)
    return start_date + timedelta(days=random_days, seconds=random_seconds)

def generate_description(category):
    """Generate a concise description under 80 characters"""
    max_length = category["max_length"]
    max_beam = category["max_beam"]
    price_per_day = category["base_price"] / 100  # Convert cents to dollars
    
    # Create concise description based on vessel size
    if max_length <= 25:
        return f"Compact slip for vessels up to {max_length}'L x {max_beam}'B - ${price_per_day:.0f}/day"
    elif max_length <= 35:
        return f"Medium slip for vessels up to {max_length}'L x {max_beam}'B - ${price_per_day:.0f}/day"
    elif max_length <= 50:
        return f"Large slip for vessels up to {max_length}'L x {max_beam}'B - ${price_per_day:.0f}/day"
    elif max_length <= 75:
        return f"Luxury slip for yachts up to {max_length}'L x {max_beam}'B - ${price_per_day:.0f}/day"
    elif max_length <= 100:
        return f"Super yacht slip up to {max_length}'L x {max_beam}'B - ${price_per_day:.0f}/day"
    else:
        return f"Ultra luxury slip up to {max_length}'L x {max_beam}'B - ${price_per_day:.0f}/day"

# Set seed for reproducible results
random.seed(42)

# Remove duplicates and get unique slip classifications
unique_categories = remove_duplicates(slip_categories)
total_records = len(unique_categories)

# Generate the SQL file
print(f'Generating SlipClassification SQL file with {total_records} unique records...')

with open('slip_classifications.sql', 'w') as f:
    f.write('-- SlipClassification Data Generation Script\n')
    f.write(f'-- Generates {total_records} unique slip classification records\n')
    f.write('-- Based on unique combinations of MaxLength and MaxBeam\n')
    f.write('-- Covers vessel sizes from 18ft runabouts to 150ft+ ultra luxury yachts\n')
    f.write('-- Pricing is based on slip dimensions and amenities\n')
    f.write('-- Base prices are in cents (integer field) for daily rental rates\n\n')
    f.write('BEGIN;\n\n')
    
    f.write('INSERT INTO skipper.slip_classifications ("Name", "MaxLength", "MaxBeam", "BasePrice", "Description", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES\n')
    
    records = []
    for i, category in enumerate(unique_categories):
        name = category["name"]
        max_length = category["max_length"]
        max_beam = category["max_beam"]
        base_price = category["base_price"]
        description = generate_description(category)
        timestamp = generate_timestamp()
        timestamp_str = timestamp.strftime('%Y-%m-%d %H:%M:%S+00')
        
        # Escape single quotes in description for SQL
        description_escaped = description.replace("'", "''")
        
        record = f"('{name}', {max_length}, {max_beam}, {base_price}, '{description_escaped}', '{timestamp_str}', '{timestamp_str}', false)"
        records.append(record)
    
    f.write(',\n'.join(records))
    f.write(';\n\n')
    
    f.write('COMMIT;\n\n')
    f.write('-- Script Summary:\n')
    f.write(f'-- Total Records: {total_records}\n')
    f.write('-- Size Range: 18ft - 150ft vessels\n')
    f.write('-- Beam Range: 7ft - 25ft vessels\n')
    f.write('-- Price Range: $25.00 - $1,045.00 per day\n')
    f.write('-- Categories: Compact, Medium, Large, Luxury, Super Yacht, Ultra Luxury\n')
    f.write('-- All records have IsDeleted = false\n')
    f.write('-- Timestamps distributed between 2020-2024\n')
    f.write('-- Descriptions are concise and under 80 characters\n')
    f.write('-- Only unique MaxLength/MaxBeam combinations included\n')

print(f'SlipClassification SQL file generated: slip_classifications.sql')
print(f'File contains {total_records} unique slip classification records')
print(f'Removed {len(slip_categories) - total_records} duplicate dimension combinations')
print('Descriptions are now concise and under 80 characters') 