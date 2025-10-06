-- ============================================
-- Tabla: micro_users
-- Descripción: Almacena información de usuarios del sistema
-- ============================================
DROP TABLE IF EXISTS micro_reservas;
DROP TABLE IF EXISTS micro_airbnbs;
DROP TABLE IF EXISTS micro_users;

CREATE TABLE micro_users (
    user_id BIGINT NOT NULL,
    name VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'user',
    password VARCHAR(80) NOT NULL,
    email VARCHAR(200) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id),
    UNIQUE KEY uk_email (email)
);

-- ============================================
-- Tabla: micro_airbnbs
-- Descripción: Almacena información de alojamientos
-- ============================================
CREATE TABLE micro_airbnbs (
    id VARCHAR(255) NOT NULL,
    name VARCHAR(255) NOT NULL,
    host_id VARCHAR(255) NOT NULL,
    host_name VARCHAR(255) NOT NULL,
    neighbourhood_group VARCHAR(255) NOT NULL,
    neighbourhood VARCHAR(255) NOT NULL,
    latitude VARCHAR(255) NOT NULL,
    longitude VARCHAR(255) NOT NULL,
    room_type VARCHAR(255) NOT NULL,
    price VARCHAR(255) NOT NULL,
    minimum_nights VARCHAR(255) NOT NULL,
    number_of_reviews VARCHAR(255) NOT NULL,
    rating VARCHAR(255) NOT NULL,
    rooms VARCHAR(255) NOT NULL,
    beds VARCHAR(255) NOT NULL,
    bathrooms VARCHAR(255) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    INDEX idx_host_id (host_id),
    INDEX idx_room_type (room_type)
);

-- ============================================
-- Tabla: micro_reservas
-- Descripción: Almacena las reservas realizadas por los clientes
-- ============================================
CREATE TABLE micro_reservas (
    reservation_id INT(11) NOT NULL AUTO_INCREMENT,
    airbnb_id VARCHAR(255) NOT NULL,
    airbnb_name VARCHAR(255) NOT NULL,
    host_id VARCHAR(255) NOT NULL,
    client_id VARCHAR(255) NOT NULL,
    client_name VARCHAR(255) NOT NULL,
    reservation_date DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (reservation_id),
    INDEX idx_client_id (client_id),
    INDEX idx_host_id (host_id),
    INDEX idx_airbnb_id (airbnb_id)
);