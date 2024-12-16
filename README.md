# **Project Overview: SoundIt**

**SoundIt** is a C# application that processes YouTube video URLs to extract the medium-quality audio stream URL and the corresponding video title. It achieves this by:

## **Features and Workflow**

1. **URL Validation**  
   - Uses a regex pattern to ensure the input URL is a valid YouTube video link.

2. **API Communication**  
   - Sends an HTTP POST request to YouTube's internal API with a structured payload to fetch video details.

3. **Audio Quality Handling**  
   - Filters and prioritizes audio streams with "AUDIO_QUALITY_MEDIUM" from the API response.

4. **Async Operations**  
   - Implements asynchronous processing for HTTP requests and JSON parsing for better performance.

## **How It Works**
1. The application validates and extracts the YouTube video ID from the provided URL using a regular expression.
2. It sends a POST request to a YouTube API endpoint with the extracted video ID and a predefined payload.
3. The JSON response is parsed to retrieve:
   - The **audio stream URL**.
   - The **title of the video**.
4. These two elements are returned as output.

## **Key Methods**
- **`isCorrectPattern(string s): string`**  
  Validates the input URL and extracts the YouTube video ID using a regex pattern.

- **`AddPayload(string id): void`**  
  Constructs the payload required for the API request, including client context information.

- **`MakeRequest(string id): Task<JObject>`**  
  Sends an HTTP POST request to the YouTube API and parses the JSON response.

- **`Execute(): Task<string[]>`**  
  Orchestrates the process of validation, API communication, and extraction of audio and title data.

## **Use Case**
This application is ideal for scenarios where users need to extract audio and metadata from YouTube videos programmatically, such as:
- Creating audio-only playlists.
- Integrating with other media applications.
