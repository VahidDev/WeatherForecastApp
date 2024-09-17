# Weather Microservice

A .NET 8 microservice that aggregates weather forecasts from multiple third-party APIs, with caching implemented using Redis. The service provides unified weather data to clients, reducing redundant API calls and improving performance.


## Overview

The Weather Microservice is designed to provide weather forecasts by aggregating data from multiple third-party APIs:
- **OpenWeatherMap**
- **Weatherbit**
- **WeatherAPI**

The service uses Redis for caching responses from these APIs to improve performance and reduce the number of external API calls.


## Architecture

- **Controllers**
  - `WeatherForecastController`: Handles incoming HTTP requests and responses.
- **Services**
  - `WeatherForecastService`: Coordinates calls to third-party APIs and manages caching.
  - **Third-Party API Services**: Individual services that fetch data from specific APIs.
    - `OpenWeatherApiService`
    - `WeatherbitApiService`
    - `WeatherApiService`
  - `RedisCacheService`: Centralized caching service using Redis.
- **Models**
  - Data models representing requests, responses, and configuration options.
- **Caching**
  - **ICacheService**: Interface for caching operations.
  - **RedisCacheService**: Implementation of `ICacheService` using Redis.
- **Docker and Docker Compose**
  - Containerization of the application and Redis for easy deployment.


## Prerequisites

- **Docker**: Install Docker from [https://www.docker.com/get-started](https://www.docker.com/get-started).
- **Docker Compose**: Comes bundled with Docker Desktop.
- **API Keys**: You need API keys from the following services:
  - [OpenWeatherMap API Key](https://home.openweathermap.org/users/sign_up)
  - [Weatherbit API Key](https://www.weatherbit.io/account/create)
  - [WeatherAPI Key](https://www.weatherapi.com/signup.aspx)


## Getting Started

### Clone the Repository
```bash
git clone https://github.com/yourusername/weather-microservice.git
cd weather-microservice
```

### Set Up Environment Variables

```env
# OpenWeatherMap API
OpenWeather_BaseUrl=https://api.openweathermap.org/data/2.5/forecast
OpenWeather_ApiKey=YOUR_OPENWEATHER_API_KEY

# Weatherbit API
Weatherbit_BaseUrl=https://api.weatherbit.io/v2.0/forecast/daily
Weatherbit_ApiKey=YOUR_WEATHERBIT_API_KEY

# WeatherAPI
WeatherApi_BaseUrl=http://api.weatherapi.com/v1/forecast.json
WeatherApi_ApiKey=YOUR_WEATHERAPI_KEY

# Redis Configuration
Redis__ConnectionString=redis:6379
```

**Note**: Replace `YOUR_OPENWEATHER_API_KEY`, `YOUR_WEATHERBIT_API_KEY`, and `YOUR_WEATHERAPI_KEY` with your actual API keys.

### Build and Run with Docker Compose

Ensure Docker is running on your machine, then execute:
```bash
docker-compose up --build
```

This command builds the Docker images and starts the services defined in `docker-compose.yml`.

## Usage

Once the application is running, you can access the API to get weather forecasts.

### API Endpoint

```http
GET http://localhost:8080/api/WeatherForecast
```

#### Query Parameters:

- `Date` (required): The date for which you want the forecast (format: `YYYY-MM-DD`).
- `City` (required): The city name.
- `Country` (required): The country code (e.g., `US`, `UK`).

### Example Request

```bash
curl "http://localhost:8080/api/WeatherForecast?Date=2023-10-15&City=London&Country=UK"
```

### Example Response

```json
[
  {
    "source": "OpenWeatherMap",
    "data": {
      "source": "OpenWeatherMap",
      "date": "2023-10-15T00:00:00Z",
      "temperatureCelsius": 15.0,
      "weatherDescription": "Light rain"
    },
    "error": null
  },
  {
    "source": "Weatherbit",
    "data": {
      "source": "Weatherbit",
      "date": "2023-10-15T00:00:00Z",
      "temperatureCelsius": 14.5,
      "weatherDescription": "Partly cloudy"
    },
    "error": null
  },
  {
    "source": "WeatherAPI",
    "data": {
      "source": "WeatherAPI",
      "date": "2023-10-15T00:00:00Z",
      "temperatureCelsius": 15.2,
      "weatherDescription": "Overcast"
    },
    "error": null
  }
]
```


### Note on API Data Availability
Important: Since the APIs used in this application may be accessed under free trial accounts, there might be limitations on the availability of historical or future data. Some APIs may not provide data for dates far in the past or future when using free tiers.

Please be aware that requests for certain dates may not return data due to these limitations.

## Project Structure

```
weather-microservice/
├── Controllers/
│   └── WeatherForecastController.cs
├── Models/
│   ├── WeatherForecastRequest.cs
│   ├── WeatherForecastResult.cs
│   ├── UnifiedWeatherForecast.cs
│   ├── ThirdPartyConnectionsOptions.cs
│   └── RedisOptions.cs
├── Services/
│   ├── IWeatherForecastService.cs
│   ├── WeatherForecastService.cs
│   ├── ICacheService.cs
│   ├── RedisCacheService.cs
│   └── ThirdPartyApis/
│       ├── IThirdPartyWeatherApi.cs
│       ├── OpenWeatherApiService.cs
│       ├── WeatherbitApiService.cs
│       └── WeatherApiService.cs
├── Dockerfile
├── docker-compose.yml
├── .env
└── README.md
```

## Contributing

Contributions are welcome! Please follow these steps:

1. **Fork the Repository**: Click the "Fork" button at the top right of this page.
2. **Clone Your Fork**:

   ```bash
   git clone https://github.com/yourusername/weather-microservice.git
   ```

3. **Create a Branch**:

   ```bash
   git checkout -b feature/your-feature-name
   ```

4. **Commit Your Changes**:

   ```bash
   git commit -m "Add your commit message"
   ```

5. **Push to Your Fork**:

   ```bash
   git push origin feature/your-feature-name
   ```

6. **Submit a Pull Request**: Go to the original repository and create a pull request from your fork.


## Additional Information

### Testing the Application

After starting the application with Docker Compose, you can test it using tools like `curl`, Postman, or a web browser.

- **Swagger UI**: If Swagger is enabled, you can access API documentation at:

  ```
  http://localhost:8080/swagger
  ```

### Stopping the Application

To stop the application and remove containers, run:

```bash
docker-compose down
```

### Customization

- **Adjust Cache Expiration**: Modify the expiration time for cached data in the services if needed.
- **Add More APIs**: You can extend the application by adding more third-party weather APIs. Implement the `IThirdPartyWeatherApi` interface for new services.

---

## Acknowledgments

- Thanks to the providers of the third-party APIs for their services.
- Appreciation to the open-source community for valuable resources and support.

---

Let me know if you need any further assistance or additional customization for your `README.md` file!