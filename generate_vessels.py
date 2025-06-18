import random
from datetime import datetime, timedelta

# Define vessel name components
name_prefixes = ['Sea', 'Ocean', 'Wave', 'Wind', 'Storm', 'Thunder', 'Lightning', 'Coral', 'Pearl', 'Crystal', 'Golden', 'Silver', 'Blue', 'Deep', 'Royal', 'Majestic', 'Swift', 'Mighty', 'Bold', 'Brave', 'Wild', 'Free', 'Grand', 'Noble', 'Proud', 'Strong', 'Fast', 'True', 'Pure', 'Bright', 'Ancient', 'Modern', 'Classic', 'Vintage', 'Elite', 'Prime', 'Ultra', 'Super', 'Mega', 'Epic']

name_suffixes = ['Breeze', 'Dream', 'Runner', 'Glory', 'Star', 'Belle', 'Chaser', 'Horizon', 'Bay', 'Walker', 'Reef', 'Gate', 'Stream', 'Bell', 'Isle', 'Dance', 'Magic', 'Wind', 'Valley', 'Sound', 'Largo', 'Monica', 'Peak', 'Spirit', 'Treasure', 'Coast', 'Thunder', 'Falls', 'Pride', 'City', 'Quest', 'Adventure', 'Explorer', 'Voyager', 'Seeker', 'Hunter', 'Raider', 'Warrior', 'Guardian', 'Champion']

geographic_names = ['Miami', 'Malibu', 'Newport', 'Hamptons', 'Nantucket', 'Martha', 'Catalina', 'Coronado', 'Sausalito', 'Monterey', 'Carmel', 'Bodega', 'Mendocino', 'Capitola', 'Tiburon', 'Belvedere', 'Annapolis', 'Camden', 'Bar Harbor', 'Kennebunkport', 'Mystic', 'Greenwich', 'Sag Harbor', 'Shelter Island', 'Block Island', 'Hilton Head', 'Kiawah', 'Amelia', 'Marco', 'Sanibel']

# All US states and territories for vessel registration
states = [
    'AL', 'AK', 'AZ', 'AR', 'CA', 'CO', 'CT', 'DE', 'FL', 'GA',
    'HI', 'ID', 'IL', 'IN', 'IA', 'KS', 'KY', 'LA', 'ME', 'MD',
    'MA', 'MI', 'MN', 'MS', 'MO', 'MT', 'NE', 'NV', 'NH', 'NJ',
    'NM', 'NY', 'NC', 'ND', 'OH', 'OK', 'OR', 'PA', 'RI', 'SC',
    'SD', 'TN', 'TX', 'UT', 'VT', 'VA', 'WA', 'WV', 'WI', 'WY',
    'PR', 'VI', 'AS', 'GU', 'MP'  # Territories
]

letters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'

def generate_timestamp():
    start_date = datetime(2014, 1, 1)
    end_date = datetime(2024, 12, 31)
    time_between = end_date - start_date
    days_between = time_between.days
    random_days = random.randrange(days_between)
    random_seconds = random.randrange(86400)
    return start_date + timedelta(days=random_days, seconds=random_seconds)

def generate_vessel_name():
    name_type = random.choice(['prefix_suffix', 'geographic', 'creative'])
    
    if name_type == 'prefix_suffix':
        return f'{random.choice(name_prefixes)} {random.choice(name_suffixes)}'
    elif name_type == 'geographic':
        return f'{random.choice(geographic_names)} {random.choice(name_suffixes)}'
    else:
        return f'{random.choice(name_prefixes)} {random.choice(geographic_names)}'

def generate_length():
    return round(random.uniform(18.0, 50.0), 1)

def generate_beam():
    return round(random.uniform(6.0, 15.0), 1)

def generate_registration(batch, record_num):
    state = random.choice(states)
    return f'{state}{batch}{record_num:03d}{random.choice(letters)}{random.choice(letters)}'

# Set seed for reproducible results
random.seed(42)

# Generate the complete SQL file
print('Generating complete SQL file with 5000 records...')

with open('test_vessels_complete.sql', 'w') as f:
    f.write('-- Complete Test Vessel Data Generation Script\n')
    f.write('-- Generates 5000 unique vessel records in 10 batches of 500 records each\n')
    f.write('-- Registration numbers follow standard US vessel registration format\n')
    f.write('-- Includes all 50 US states plus territories for registration\n')
    f.write('-- Timestamps are randomly distributed over the last decade (2014-2024)\n\n')
    f.write('BEGIN;\n\n')
    
    used_registrations = set()
    used_names = set()
    
    for batch in range(1, 11):
        print(f'Generating Batch {batch}: Records {(batch-1)*500 + 1}-{batch*500}')
        f.write(f'-- Batch {batch}: Records {(batch-1)*500 + 1}-{batch*500}\n')
        f.write('INSERT INTO skipper.vessels ("RegistrationNumber", "Name", "Length", "Beam", "VesselType", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES\n')
        
        batch_records = []
        for i in range(500):
            record_num = i + 1
            
            # Ensure unique registration number
            attempts = 0
            while True:
                reg_num = generate_registration(batch, record_num)
                if reg_num not in used_registrations:
                    used_registrations.add(reg_num)
                    break
                attempts += 1
                if attempts > 100:
                    # If we can't find a unique registration after 100 attempts,
                    # use a sequential approach
                    reg_num = f'{random.choice(states)}{batch}{len(used_registrations):04d}{random.choice(letters)}{random.choice(letters)}'
                    used_registrations.add(reg_num)
                    break
            
            # Ensure unique vessel name
            attempts = 0
            while True:
                name = generate_vessel_name()
                if name not in used_names:
                    used_names.add(name)
                    break
                attempts += 1
                if attempts > 100:
                    # If we can't find a unique name after 100 attempts,
                    # append a number to make it unique
                    name = f'{name} {len(used_names) + 1}'
                    used_names.add(name)
                    break
            
            length = generate_length()
            beam = generate_beam()
            vessel_type = 'Motorboat' if i % 2 == 0 else 'Sailboat'
            timestamp = generate_timestamp()
            timestamp_str = timestamp.strftime('%Y-%m-%d %H:%M:%S+00')
            
            record = f"('{reg_num}', '{name}', {length}, {beam}, '{vessel_type}', '{timestamp_str}', '{timestamp_str}', false)"
            batch_records.append(record)
        
        f.write(',\n'.join(batch_records))
        f.write(';\n\n')
    
    f.write('COMMIT;\n\n')
    f.write('-- Script Summary:\n')
    f.write('-- Total Records: 5000\n')
    f.write('-- Batches: 10 (500 records each)\n')
    f.write('-- Registration Format: State + Batch + Sequential + 2 Letters\n')
    f.write('-- States Included: All 50 US states plus territories (AL-WY, PR, VI, AS, GU, MP)\n')
    f.write('-- Vessel Types: Alternating Motorboat/Sailboat\n')
    f.write('-- Length Range: 18.0 - 50.0 feet\n')
    f.write('-- Beam Range: 6.0 - 15.0 feet\n')
    f.write('-- Date Range: 2014-2024\n')
    f.write('-- All records have IsDeleted = false\n')
    f.write('-- All registration numbers and vessel names are unique\n')

print('Complete SQL file generated: test_vessels_complete.sql')
print(f'File contains 5000 unique vessel records from {len(states)} states/territories in 10 batches of 500 records each.')
print(f'States included: {", ".join(states)}') 