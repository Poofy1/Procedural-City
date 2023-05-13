# Unity Low Poly Procedural City Documentation

This project is a optimized low poly procedural city generator built in Unity. It is designed to be used as an alternative to night city skyboxes. The generator creates a city using configurable parameters such as city width, city length, block width, block length, road width, and building height. It also includes the ability to add cars and antennas to the generated city.

## How to Use

1. Import the project into Unity.
2. Open the `City` scene.
3. Adjust the parameters in the `CityGenerator` script to your desired values.
4. Run the scene to generate the city.

## Parameters

### City Config

- `cityWidth`: The width of the city in blocks.
- `cityLength`: The length of the city in blocks.
- `blockWidth`: The width of each block in the city.
- `blockLength`: The length of each block in the city.
- `roadWidth`: The width of each road in the city.
- `heightFrequency`: The frequency of the building height perlin noise.
- `heightFrequency_small`: The frequency of the small building height perlin noise.

### Building Config

- `buildingSize`: The size of each building in the city.
- `buildingMax`: The maximum height of each building in the city.
- `buildingMin`: The minimum height of each building in the city.
- `skyScraperThreshold`: The threshold for buildings to be considered skyscrapers.
- `antennaProbability`: The probability of an antenna being added to a skyscraper.

### Car Config

- `cars1`: The prefab for the first type of car.
- `cars2`: The prefab for the second type of car.
