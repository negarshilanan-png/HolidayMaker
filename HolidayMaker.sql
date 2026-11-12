DROP DATABASE IF EXISTS HolidayMaker;
CREATE DATABASE HolidayMaker;
USE HolidayMaker;

-- 1. Skapa tabeller baserat på E/R-diagrammet
CREATE TABLE `Hotels`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL,
    `location` VARCHAR(255) NOT NULL,
    `distance_to_beach` INT NOT NULL CHECK (`distance_to_beach` >= 0),
    `distance_to_city` INT NOT NULL CHECK (`distance_to_city` >= 0),
    `rating` DECIMAL(8, 2) NOT NULL CHECK (`rating` >= 1.00 AND `rating` <= 5.00)
);

CREATE TABLE `Customers`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL,
    `address` VARCHAR(255) NOT NULL,
    `email` VARCHAR(255) NOT NULL,
    `phone` VARCHAR(255) NOT NULL
);
ALTER TABLE `Customers` ADD UNIQUE `customers_email_unique`(`email`);

CREATE TABLE `Rooms`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `id_price_per_night` DECIMAL(8, 2) NOT NULL CHECK (`id_price_per_night` > 0),
    `capacity` INT NOT NULL CHECK (`capacity` > 0),
    `hotel_id` BIGINT UNSIGNED NOT NULL,
    `room_number` INT NOT NULL,
    UNIQUE (`hotel_id`, `room_number`)
);

CREATE TABLE `Bookings`(
    `booking_number` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `customer_id` BIGINT UNSIGNED NOT NULL,
    `room_id` BIGINT UNSIGNED NOT NULL,
    `start_date` DATE NOT NULL,
    `end_date` DATE NOT NULL,
    `total_price` DECIMAL(8, 2) NOT NULL CHECK (`total_price` >= 0),
    `is_cancelled` BOOLEAN NOT NULL DEFAULT FALSE,
    `is_booked` BOOLEAN NOT NULL DEFAULT TRUE,
    `number_of_occupant` INT NOT NULL CHECK (`number_of_occupant` > 0),
    CHECK (`end_date` > `start_date`)
);

CREATE TABLE `Amenities`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE `Hotel_x_Amenities`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `hotel_id` BIGINT UNSIGNED NOT NULL,
    `amenities_id` BIGINT UNSIGNED NOT NULL,
    UNIQUE (`hotel_id`, `amenities_id`)
);

-- Främmande nycklar
ALTER TABLE `Hotel_x_Amenities` ADD CONSTRAINT `hotel_x_amenities_amenities_id_foreign` FOREIGN KEY(`amenities_id`) REFERENCES `Amenities`(`id`) ON DELETE CASCADE;
ALTER TABLE `Rooms` ADD CONSTRAINT `rooms_hotel_id_foreign` FOREIGN KEY(`hotel_id`) REFERENCES `Hotels`(`id`) ON DELETE CASCADE;
ALTER TABLE `Hotel_x_Amenities` ADD CONSTRAINT `hotel_x_amenities_hotel_id_foreign` FOREIGN KEY(`hotel_id`) REFERENCES `Hotels`(`id`) ON DELETE CASCADE;
ALTER TABLE `Bookings` ADD CONSTRAINT `bookings_room_id_foreign` FOREIGN KEY(`room_id`) REFERENCES `Rooms`(`id`) ON DELETE CASCADE;
ALTER TABLE `Bookings` ADD CONSTRAINT `bookings_customer_id_foreign` FOREIGN KEY(`customer_id`) REFERENCES `Customers`(`id`) ON DELETE CASCADE;

-- 2. Testdata (5 boenden, 20 rum, 5 kunder, 25 bokningar)
INSERT INTO `Amenities` (`id`, `name`) VALUES
(1, 'Pool'),
(2, 'Restaurang');

INSERT INTO `Hotels` (`id`, `name`, `location`, `distance_to_beach`, `distance_to_city`, `rating`) VALUES
(1, 'Strandhotellet', 'Halmstad', 50, 1200, 4.50),
(2, 'Hotell Central', 'Halmstad', 1500, 100, 3.80),
(3, 'Kustens Pärla', 'Malmö', 200, 500, 4.80),
(4, 'City Pensionat', 'Malmö', 3000, 50, 3.50),
(5, 'Grand Hotel', 'Malmö', 800, 200, 4.20);

INSERT INTO `Hotel_x_Amenities` (`hotel_id`, `amenities_id`) VALUES
(1, 1), (1, 2), -- Strandhotellet: Pool + Restaurang
(2, 2), -- Hotell Central: endast Restaurang
(3, 1), (3, 2), -- Kustens Pärla: Pool + Restaurang
(5, 1); -- Grand Hotel: endast Pool

INSERT INTO `Customers` (`id`, `name`, `address`, `email`, `phone`) VALUES
(1, 'Erik Svensson', 'Storgatan 1, Halmstad', 'erik@example.se', '0701234567'),
(2, 'Anna Karlsson', 'Strandvägen 4, Malmö', 'anna@example.se', '0739876543'),
(3, 'Johan Nilsson', 'Parkgatan 12, Halmstad', 'johan@example.se', '0721112233'),
(4, 'Maria Larsson', 'Kungsgatan 8, Malmö', 'maria@example.se', '0765554433'),
(5, 'Karin Olsson', 'Hamngatan 3, Malmö', 'karin@example.se', '0709998877');

INSERT INTO `Rooms` (`id`, `hotel_id`, `room_number`, `capacity`, `id_price_per_night`) VALUES
(1, 1, 101, 1, 800.00), (2, 1, 102, 2, 1200.00), (3, 1, 103, 4, 2000.00), (4, 1, 104, 2, 1300.00),
(5, 2, 201, 1, 650.00), (6, 2, 202, 2, 950.00), (7, 2, 203, 4, 1600.00), (8, 2, 204, 2, 1000.00),
(9, 3, 301, 1, 1100.00), (10, 3, 302, 2, 1700.00),(11, 3, 303, 4, 2800.00),(12, 3, 304, 2, 1800.00),
(13, 4, 401, 1, 500.00), (14, 4, 402, 2, 800.00), (15, 4, 403, 4, 1300.00),(16, 4, 404, 1, 550.00),
(17, 5, 501, 1, 950.00), (18, 5, 502, 2, 1500.00), (19, 5, 503, 4, 2400.00),(20, 5, 504, 2, 1600.00);

INSERT INTO `Bookings` (`customer_id`, `room_id`, `start_date`, `end_date`, `total_price`, `is_cancelled`, `is_booked`, `number_of_occupant`) VALUES
(1, 1, '2026-12-01', '2026-12-04', 2400.00, FALSE, TRUE, 1),
(2, 2, '2026-12-01', '2026-12-05', 4800.00, FALSE, TRUE, 2),
(3, 3, '2026-12-02', '2026-12-06', 8000.00, FALSE, TRUE, 4),
(4, 5, '2026-12-03', '2026-12-07', 2600.00, FALSE, TRUE, 1),
(5, 6, '2026-12-04', '2026-12-08', 3800.00, FALSE, TRUE, 2),
(1, 7, '2026-12-05', '2026-12-10', 8000.00, FALSE, TRUE, 3),
(2, 9, '2026-12-06', '2026-12-09', 3300.00, FALSE, TRUE, 1),
(3, 10, '2026-12-07', '2026-12-11', 6800.00, FALSE, TRUE, 2),
(4, 11, '2026-12-08', '2026-12-12', 11200.00, FALSE, TRUE, 4),
(5, 13, '2026-12-09', '2026-12-12', 1500.00, FALSE, TRUE, 1),
(1, 14, '2026-12-10', '2026-12-14', 3200.00, FALSE, TRUE, 2),
(2, 15, '2026-12-11', '2026-12-15', 5200.00, FALSE, TRUE, 3),
(3, 17, '2026-12-12', '2026-12-15', 2850.00, FALSE, TRUE, 1),
(4, 18, '2026-12-13', '2026-12-17', 6000.00, FALSE, TRUE, 2),
(5, 19, '2026-12-14', '2026-12-18', 9600.00, FALSE, TRUE, 4),
(1, 20, '2026-12-15', '2026-12-19', 6400.00, FALSE, TRUE, 2),
(2, 1, '2026-12-16', '2026-12-20', 3200.00, FALSE, TRUE, 1),
(3, 2, '2026-12-17', '2026-12-21', 4800.00, FALSE, TRUE, 2),
(4, 3, '2026-12-18', '2026-12-22', 8000.00, FALSE, TRUE, 4),
(5, 5, '2026-12-19', '2026-12-23', 2600.00, FALSE, TRUE, 1),
(1, 6, '2026-12-20', '2026-12-24', 3800.00, FALSE, TRUE, 2),
(2, 7, '2026-12-21', '2026-12-25', 6400.00, FALSE, TRUE, 3),
(3, 9, '2026-12-22', '2026-12-26', 4400.00, FALSE, TRUE, 1),
(4, 10, '2026-12-23', '2026-12-27', 6800.00, FALSE, TRUE, 2),
(5, 11, '2026-12-24', '2026-12-28', 11200.00, FALSE, TRUE, 4);
