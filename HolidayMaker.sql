CREATE DATABASE IF NOT EXISTS HolidayMaker;
USE HolidayMaker;

CREATE TABLE `Hotels`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL,
    `location` VARCHAR(255) NOT NULL,
    `distance_to_beach` INT NOT NULL,
    `distance_to_city` INT NOT NULL,
    `rating` DECIMAL(3, 1) NULL
);

CREATE TABLE `Customers`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL,
    `address` VARCHAR(255) NULL,
    `email` VARCHAR(255) NOT NULL UNIQUE,
    `phone` VARCHAR(255) NULL
);

CREATE TABLE `Rooms`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `hotel_id` BIGINT UNSIGNED NOT NULL,
    `room_number` VARCHAR(10) NOT NULL,
    `capacity` INT NOT NULL,
    `price_per_night` DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (`hotel_id`) REFERENCES `Hotels`(`id`) ON DELETE CASCADE
);

CREATE TABLE `Amenities`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL
);

CREATE TABLE `Hotel_x_Amenities`(
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `hotel_id` BIGINT UNSIGNED NOT NULL,
    `amenities_id` BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (`hotel_id`) REFERENCES `Hotels`(`id`) ON DELETE CASCADE,
    FOREIGN KEY (`amenities_id`) REFERENCES `Amenities`(`id`) ON DELETE CASCADE
);

CREATE TABLE `Bookings`(
    `booking_number` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `customer_id` BIGINT UNSIGNED NOT NULL,
    `room_id` BIGINT UNSIGNED NOT NULL,
    `start_date` DATE NOT NULL,
    `end_date` DATE NOT NULL,
    `total_price` DECIMAL(10, 2) NOT NULL,
    `is_cancelled` BOOLEAN NOT NULL DEFAULT FALSE,
    `number_of_occupant` INT NOT NULL,
    FOREIGN KEY (`customer_id`) REFERENCES `Customers`(`id`) ON DELETE CASCADE,
    FOREIGN KEY (`room_id`) REFERENCES `Rooms`(`id`) ON DELETE CASCADE
);

-- داده‌های تست سوئدی (Testdata på svenska)
INSERT INTO `Hotels` (`name`, `location`, `distance_to_beach`, `distance_to_city`, `rating`) VALUES
('Strandhotellet', 'Halmstad', 50, 1200, 4.5),
('Hotell Central', 'Göteborg', 8000, 200, 4.2),
('Kustens Pärla', 'Malmö', 300, 1500, 4.8);

INSERT INTO `Amenities` (`name`) VALUES
('Pool'),
('WiFi'),
('Frukost'),
('Parkering'),
('Bastu');

INSERT INTO `Hotel_x_Amenities` (`hotel_id`, `amenities_id`) VALUES
(1, 1), (1, 2), (1, 3),
(2, 2), (2, 3),
(3, 1), (3, 2), (3, 4), (3, 5);

INSERT INTO `Rooms` (`hotel_id`, `room_number`, `capacity`, `price_per_night`) VALUES
(1, '101', 1, 850.00),
(1, '102', 2, 1350.00),
(2, '201', 2, 1100.00),
(2, '202', 4, 2100.00),
(3, '301', 2, 1600.00);

INSERT INTO `Customers` (`name`, `address`, `email`, `phone`) VALUES
('Erik Svensson', 'Storgatan 12, Halmstad', 'erik.svensson@example.se', '0701234567'),
('Anna Karlsson', 'Vasagatan 4, Göteborg', 'anna.karlsson@example.se', '0739876543');
