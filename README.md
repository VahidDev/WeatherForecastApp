# Weather Microservice

A .NET 8 microservice that aggregates weather forecasts from multiple third-party APIs, with caching. The service provides unified weather data to clients, reducing redundant API calls and improving performance.


## Overview

The Weather Microservice is designed to provide weather forecasts by aggregating data from multiple third-party APIs:
- **OpenWeatherMap**
- **Weatherbit**
- **WeatherAPI**

The service uses caching to improve performance and reduce the number of external API calls.

## GET Request  

https://weatherapp123-gwd4eud6bmamfnex.westeurope-01.azurewebsites.net/api/WeatherForecast?city=Baku&country=Azerbaijan&date=2024-09-17 

(See postman collection in the repo)

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
