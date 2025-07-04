-- SlipClassification Data Generation Script
-- Generates 38 unique slip classification records
-- Based on unique combinations of MaxLength and MaxBeam
-- Covers vessel sizes from 18ft runabouts to 150ft+ ultra luxury yachts
-- Pricing is based on slip dimensions and amenities
-- Base prices are in cents (integer field) for daily rental rates

BEGIN;

INSERT INTO skipper.slip_classifications ("Name", "MaxLength", "MaxBeam", "BasePrice", "Description", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES
('Compact Runabout', 18.0, 7.0, 2500, 'Compact slip for vessels up to 18.0''L x 7.0''B - $25/day', '2023-08-02 04:03:12+00', '2023-08-02 04:03:12+00', false),
('Small Bowrider', 20.0, 7.5, 2800, 'Compact slip for vessels up to 20.0''L x 7.5''B - $28/day', '2020-02-21 10:00:48+00', '2020-02-21 10:00:48+00', false),
('Small Cuddy Cabin', 22.0, 8.0, 3200, 'Compact slip for vessels up to 22.0''L x 8.0''B - $32/day', '2021-05-16 08:07:36+00', '2021-05-16 08:07:36+00', false),
('Small Cruiser', 24.0, 8.5, 3600, 'Compact slip for vessels up to 24.0''L x 8.5''B - $36/day', '2020-10-12 03:43:54+00', '2020-10-12 03:43:54+00', false),
('Small Sport Boat', 25.0, 8.5, 3800, 'Compact slip for vessels up to 25.0''L x 8.5''B - $38/day', '2023-10-17 19:51:22+00', '2023-10-17 19:51:22+00', false),
('Medium Bowrider', 26.0, 9.0, 4200, 'Medium slip for vessels up to 26.0''L x 9.0''B - $42/day', '2020-06-27 21:29:57+00', '2020-06-27 21:29:57+00', false),
('Medium Express', 28.0, 9.5, 4800, 'Medium slip for vessels up to 28.0''L x 9.5''B - $48/day', '2022-05-14 01:09:25+00', '2022-05-14 01:09:25+00', false),
('Medium Sport Yacht', 30.0, 10.0, 5600, 'Medium slip for vessels up to 30.0''L x 10.0''B - $56/day', '2020-03-02 03:24:40+00', '2020-03-02 03:24:40+00', false),
('Large Express Cruiser', 33.0, 10.5, 6800, 'Medium slip for vessels up to 33.0''L x 10.5''B - $68/day', '2021-03-23 08:28:15+00', '2021-03-23 08:28:15+00', false),
('Large Sport Yacht', 34.0, 11.0, 7200, 'Medium slip for vessels up to 34.0''L x 11.0''B - $72/day', '2022-10-31 21:55:07+00', '2022-10-31 21:55:07+00', false),
('Large Cabin Cruiser', 35.0, 11.0, 7600, 'Medium slip for vessels up to 35.0''L x 11.0''B - $76/day', '2020-02-24 20:26:03+00', '2020-02-24 20:26:03+00', false),
('Extra Large Express', 36.0, 11.5, 8200, 'Large slip for vessels up to 36.0''L x 11.5''B - $82/day', '2021-02-11 23:39:41+00', '2021-02-11 23:39:41+00', false),
('Extra Large Sport Yacht', 38.0, 12.0, 9200, 'Large slip for vessels up to 38.0''L x 12.0''B - $92/day', '2023-12-07 19:50:26+00', '2023-12-07 19:50:26+00', false),
('Extra Large Cabin Yacht', 40.0, 12.5, 10200, 'Large slip for vessels up to 40.0''L x 12.5''B - $102/day', '2022-05-09 08:01:33+00', '2022-05-09 08:01:33+00', false),
('Premium Motor Yacht', 42.0, 13.0, 11400, 'Large slip for vessels up to 42.0''L x 13.0''B - $114/day', '2022-07-08 21:27:16+00', '2022-07-08 21:27:16+00', false),
('Premium Sport Yacht', 44.0, 13.5, 12600, 'Large slip for vessels up to 44.0''L x 13.5''B - $126/day', '2021-07-23 00:14:11+00', '2021-07-23 00:14:11+00', false),
('Luxury Express Yacht', 46.0, 14.0, 14000, 'Large slip for vessels up to 46.0''L x 14.0''B - $140/day', '2024-04-03 05:48:46+00', '2024-04-03 05:48:46+00', false),
('Luxury Cabin Yacht', 48.0, 14.5, 15400, 'Large slip for vessels up to 48.0''L x 14.5''B - $154/day', '2023-11-30 15:23:12+00', '2023-11-30 15:23:12+00', false),
('Luxury Mega Yacht', 50.0, 15.0, 16800, 'Large slip for vessels up to 50.0''L x 15.0''B - $168/day', '2021-11-27 10:07:01+00', '2021-11-27 10:07:01+00', false),
('Super Luxury Express', 52.0, 15.0, 18200, 'Luxury slip for yachts up to 52.0''L x 15.0''B - $182/day', '2020-11-14 07:50:21+00', '2020-11-14 07:50:21+00', false),
('Super Luxury Motor Yacht', 54.0, 15.5, 19600, 'Luxury slip for yachts up to 54.0''L x 15.5''B - $196/day', '2024-04-12 12:15:18+00', '2024-04-12 12:15:18+00', false),
('Super Luxury Cabin Yacht', 56.0, 16.0, 21200, 'Luxury slip for yachts up to 56.0''L x 16.0''B - $212/day', '2020-07-28 03:22:36+00', '2020-07-28 03:22:36+00', false),
('Super Luxury Mega Yacht', 60.0, 16.5, 24200, 'Luxury slip for yachts up to 60.0''L x 16.5''B - $242/day', '2022-02-17 03:31:16+00', '2022-02-17 03:31:16+00', false),
('Mega Yacht Standard', 65.0, 17.0, 27000, 'Luxury slip for yachts up to 65.0''L x 17.0''B - $270/day', '2022-01-05 12:31:22+00', '2022-01-05 12:31:22+00', false),
('Mega Yacht Deluxe', 70.0, 17.5, 30200, 'Luxury slip for yachts up to 70.0''L x 17.5''B - $302/day', '2023-05-21 09:37:51+00', '2023-05-21 09:37:51+00', false),
('Mega Yacht Premium', 75.0, 18.0, 33600, 'Luxury slip for yachts up to 75.0''L x 18.0''B - $336/day', '2024-07-10 01:34:55+00', '2024-07-10 01:34:55+00', false),
('Super Yacht Standard', 80.0, 18.5, 37500, 'Super yacht slip up to 80.0''L x 18.5''B - $375/day', '2024-02-03 16:43:37+00', '2024-02-03 16:43:37+00', false),
('Super Yacht Deluxe', 85.0, 19.0, 41800, 'Super yacht slip up to 85.0''L x 19.0''B - $418/day', '2023-01-03 04:32:41+00', '2023-01-03 04:32:41+00', false),
('Super Yacht Premium', 90.0, 19.5, 46500, 'Super yacht slip up to 90.0''L x 19.5''B - $465/day', '2022-02-14 02:52:08+00', '2022-02-14 02:52:08+00', false),
('Super Yacht Elite', 95.0, 20.0, 51600, 'Super yacht slip up to 95.0''L x 20.0''B - $516/day', '2023-02-04 10:40:27+00', '2023-02-04 10:40:27+00', false),
('Super Yacht Ultimate', 100.0, 20.5, 57200, 'Super yacht slip up to 100.0''L x 20.5''B - $572/day', '2024-08-25 22:53:17+00', '2024-08-25 22:53:17+00', false),
('Ultra Luxury Standard', 110.0, 21.0, 64500, 'Ultra luxury slip up to 110.0''L x 21.0''B - $645/day', '2023-06-20 13:10:00+00', '2023-06-20 13:10:00+00', false),
('Ultra Luxury Deluxe', 120.0, 22.0, 72800, 'Ultra luxury slip up to 120.0''L x 22.0''B - $728/day', '2023-03-28 07:00:03+00', '2023-03-28 07:00:03+00', false),
('Ultra Luxury Premium', 130.0, 23.0, 82200, 'Ultra luxury slip up to 130.0''L x 23.0''B - $822/day', '2023-12-13 02:31:56+00', '2023-12-13 02:31:56+00', false),
('Ultra Luxury Elite', 140.0, 24.0, 92800, 'Ultra luxury slip up to 140.0''L x 24.0''B - $928/day', '2020-04-03 08:17:51+00', '2020-04-03 08:17:51+00', false),
('Ultra Luxury Ultimate', 150.0, 25.0, 104500, 'Ultra luxury slip up to 150.0''L x 25.0''B - $1045/day', '2024-05-02 10:32:10+00', '2024-05-02 10:32:10+00', false),
('Charter Vessel Standard', 80.0, 18.0, 35000, 'Super yacht slip up to 80.0''L x 18.0''B - $350/day', '2020-06-12 08:28:32+00', '2020-06-12 08:28:32+00', false),
('Charter Vessel Large', 100.0, 20.0, 55000, 'Super yacht slip up to 100.0''L x 20.0''B - $550/day', '2024-11-09 03:40:38+00', '2024-11-09 03:40:38+00', false);

COMMIT;

-- Script Summary:
-- Total Records: 38
-- Size Range: 18ft - 150ft vessels
-- Beam Range: 7ft - 25ft vessels
-- Price Range: $25.00 - $1,045.00 per day
-- Categories: Compact, Medium, Large, Luxury, Super Yacht, Ultra Luxury
-- All records have IsDeleted = false
-- Timestamps distributed between 2020-2024
-- Descriptions are concise and under 80 characters
-- Only unique MaxLength/MaxBeam combinations included
