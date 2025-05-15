# REST API Plan for FiszkiNet

## 1. Resources

| Resource | Database Table | Description |
|----------|---------------|-------------|
| Users | AspNetUsers | User accounts and authentication |
| Flashcards | Flashcards | Flashcards for learning |
| StudySessions | StudySessions | Study sessions tracking |
| StudySessionDetails | StudySessionDetails | Details of flashcard reviews during study sessions |
| GenerationStatistics | GenerationStatistics | Statistics about AI-generated flashcards |

## 2. Endpoints

### Authentication Endpoints

#### Register User
- **Method**: POST
- **URL**: `/api/users/register`
- **Description**: Register a new user account
- **Request Body**:
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```
- **Response**:
  - **Success**: 201 Created
  ```json
  {
    "id": "string",
    "username": "string",
    "email": "string"
  }
  ```
  - **Error**: 
    - 400 Bad Request - Validation errors
    - 409 Conflict - Username or email already exists

#### Login
- **Method**: POST
- **URL**: `/api/users/login`
- **Description**: Authenticate a user and receive a JWT token
- **Request Body**:
```json
{
  "username": "string",
  "password": "string"
}
```
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "token": "string",
    "expiration": "datetime"
  }
  ```
  - **Error**: 
    - 400 Bad Request - Invalid credentials
    - 401 Unauthorized - Account locked or requires verification

#### Logout
- **Method**: POST
- **URL**: `/api/users/logout`
- **Description**: Invalidate the current user's token
- **Authentication**: Required
- **Response**:
  - **Success**: 204 No Content
  - **Error**: 401 Unauthorized

#### Get Current User
- **Method**: GET
- **URL**: `/api/users/current`
- **Description**: Get information about the currently authenticated user
- **Authentication**: Required
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "id": "string",
    "username": "string",
    "email": "string"
  }
  ```
  - **Error**: 401 Unauthorized

### Flashcard Endpoints

#### Get User Flashcards
- **Method**: GET
- **URL**: `/api/flashcards`
- **Description**: Get all flashcards belonging to the current user
- **Authentication**: Required
- **Query Parameters**:
  - `page` (int, optional): Page number for pagination, default 1
  - `pageSize` (int, optional): Number of items per page, default 20
  - `sortBy` (string, optional): Field to sort by (CreatedAt, UpdatedAt, NextReviewDate), default CreatedAt
  - `sortDirection` (string, optional): asc or desc, default desc
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "totalCount": 0,
    "totalPages": 0,
    "currentPage": 0,
    "pageSize": 0,
    "items": [
      {
        "id": "uuid",
        "front": "string",
        "back": "string",
        "createdAt": "datetime",
        "updatedAt": "datetime",
        "isGeneratedByAI": true,
        "easeFactor": 2.5,
        "interval": 0,
        "nextReviewDate": "datetime",
        "lastReviewDate": "datetime",
        "reviewCount": 0
      }
    ]
  }
  ```
  - **Error**: 401 Unauthorized

#### Get Flashcards Due for Review
- **Method**: GET
- **URL**: `/api/flashcards/review`
- **Description**: Get flashcards that are due for review according to the spaced repetition algorithm
- **Authentication**: Required
- **Query Parameters**:
  - `limit` (int, optional): Maximum number of flashcards to return, default 20
- **Response**:
  - **Success**: 200 OK
  ```json
  [
    {
      "id": "uuid",
      "front": "string",
      "back": "string",
      "createdAt": "datetime",
      "updatedAt": "datetime",
      "isGeneratedByAI": true,
      "easeFactor": 2.5,
      "interval": 0,
      "nextReviewDate": "datetime",
      "lastReviewDate": "datetime",
      "reviewCount": 0
    }
  ]
  ```
  - **Error**: 401 Unauthorized

#### Get Flashcard by ID
- **Method**: GET
- **URL**: `/api/flashcards/{id}`
- **Description**: Get a specific flashcard by ID
- **Authentication**: Required
- **Parameters**:
  - `id` (uuid): Flashcard ID
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "id": "uuid",
    "front": "string",
    "back": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime",
    "isGeneratedByAI": true,
    "easeFactor": 2.5,
    "interval": 0,
    "nextReviewDate": "datetime",
    "lastReviewDate": "datetime",
    "reviewCount": 0
  }
  ```
  - **Error**: 
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this flashcard
    - 404 Not Found - Flashcard not found

#### Create Flashcard
- **Method**: POST
- **URL**: `/api/flashcards`
- **Description**: Create a new flashcard
- **Authentication**: Required
- **Request Body**:
```json
{
  "front": "string",
  "back": "string",
  "isGeneratedByAI": false
}
```
- **Response**:
  - **Success**: 201 Created
  ```json
  {
    "id": "uuid",
    "front": "string",
    "back": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime",
    "isGeneratedByAI": false,
    "easeFactor": 2.5,
    "interval": 0,
    "nextReviewDate": "datetime",
    "lastReviewDate": "datetime",
    "reviewCount": 0
  }
  ```
  - **Error**: 
    - 400 Bad Request - Validation errors
    - 401 Unauthorized

#### Update Flashcard
- **Method**: PUT
- **URL**: `/api/flashcards/{id}`
- **Description**: Update an existing flashcard
- **Authentication**: Required
- **Parameters**:
  - `id` (uuid): Flashcard ID
- **Request Body**:
```json
{
  "front": "string",
  "back": "string"
}
```
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "id": "uuid",
    "front": "string",
    "back": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime",
    "isGeneratedByAI": true,
    "easeFactor": 2.5,
    "interval": 0,
    "nextReviewDate": "datetime",
    "lastReviewDate": "datetime",
    "reviewCount": 0
  }
  ```
  - **Error**: 
    - 400 Bad Request - Validation errors
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this flashcard
    - 404 Not Found - Flashcard not found

#### Delete Flashcard
- **Method**: DELETE
- **URL**: `/api/flashcards/{id}`
- **Description**: Delete a flashcard
- **Authentication**: Required
- **Parameters**:
  - `id` (uuid): Flashcard ID
- **Response**:
  - **Success**: 204 No Content
  - **Error**: 
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this flashcard
    - 404 Not Found - Flashcard not found

#### Generate Flashcards from Text
- **Method**: POST
- **URL**: `/api/flashcards/generate`
- **Description**: Generate flashcard suggestions using AI based on provided text
- **Authentication**: Required
- **Request Body**:
```json
{
  "sourceText": "string",
  "count": 10
}
```
- **Response**:
  - **Success**: 200 OK
  ```json
  [
    {
      "id": "uuid",
      "front": "string",
      "back": "string",
      "isGeneratedByAI": true
    }
  ]
  ```
  - **Error**: 
    - 400 Bad Request - Text too short/long or invalid count
    - 401 Unauthorized
    - 500 Internal Server Error - AI service unavailable

### Study Session Endpoints

#### Get User Study Sessions
- **Method**: GET
- **URL**: `/api/studysessions`
- **Description**: Get all study sessions for the current user
- **Authentication**: Required
- **Query Parameters**:
  - `page` (int, optional): Page number for pagination, default 1
  - `pageSize` (int, optional): Number of items per page, default 20
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "totalCount": 0,
    "totalPages": 0,
    "currentPage": 0,
    "pageSize": 0,
    "items": [
      {
        "id": "uuid",
        "startedAt": "datetime",
        "endedAt": "datetime",
        "flashcardsReviewed": 0
      }
    ]
  }
  ```
  - **Error**: 401 Unauthorized

#### Get Study Session by ID
- **Method**: GET
- **URL**: `/api/studysessions/{id}`
- **Description**: Get a specific study session by ID
- **Authentication**: Required
- **Parameters**:
  - `id` (uuid): Study session ID
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "id": "uuid",
    "startedAt": "datetime",
    "endedAt": "datetime",
    "flashcardsReviewed": 0
  }
  ```
  - **Error**: 
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this session
    - 404 Not Found - Session not found

#### Start Study Session
- **Method**: POST
- **URL**: `/api/studysessions/start`
- **Description**: Start a new study session
- **Authentication**: Required
- **Response**:
  - **Success**: 201 Created
  ```json
  {
    "id": "uuid",
    "startedAt": "datetime",
    "endedAt": null,
    "flashcardsReviewed": 0
  }
  ```
  - **Error**: 401 Unauthorized

#### End Study Session
- **Method**: POST
- **URL**: `/api/studysessions/{id}/end`
- **Description**: End an active study session
- **Authentication**: Required
- **Parameters**:
  - `id` (uuid): Study session ID
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "id": "uuid",
    "startedAt": "datetime",
    "endedAt": "datetime",
    "flashcardsReviewed": 5
  }
  ```
  - **Error**: 
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this session
    - 404 Not Found - Session not found
    - 409 Conflict - Session already ended

#### Get Study Session Details
- **Method**: GET
- **URL**: `/api/studysessions/{id}/details`
- **Description**: Get details of flashcard reviews in a study session
- **Authentication**: Required
- **Parameters**:
  - `id` (uuid): Study session ID
- **Response**:
  - **Success**: 200 OK
  ```json
  [
    {
      "id": "uuid",
      "flashcardId": "uuid",
      "performanceRating": 5,
      "reviewedAt": "datetime",
      "previousEaseFactor": 2.5,
      "previousInterval": 0,
      "newEaseFactor": 2.6,
      "newInterval": 1
    }
  ]
  ```
  - **Error**: 
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this session
    - 404 Not Found - Session not found

#### Record Flashcard Review
- **Method**: POST
- **URL**: `/api/studysessions/review`
- **Description**: Record a flashcard review during a study session
- **Authentication**: Required
- **Request Body**:
```json
{
  "sessionId": "uuid",
  "flashcardId": "uuid",
  "performanceRating": 5
}
```
- **Response**:
  - **Success**: 201 Created
  ```json
  {
    "id": "uuid",
    "sessionId": "uuid",
    "flashcardId": "uuid",
    "performanceRating": 5,
    "reviewedAt": "datetime",
    "previousEaseFactor": 2.5,
    "previousInterval": 0,
    "newEaseFactor": 2.6,
    "newInterval": 1
  }
  ```
  - **Error**: 
    - 400 Bad Request - Invalid rating or session already ended
    - 401 Unauthorized
    - 403 Forbidden - User doesn't own this session or flashcard
    - 404 Not Found - Session or flashcard not found

### Generation Statistics Endpoints

#### Get User Generation Statistics
- **Method**: GET
- **URL**: `/api/users/statistics`
- **Description**: Get statistics about AI-generated flashcards for the current user
- **Authentication**: Required
- **Query Parameters**:
  - `page` (int, optional): Page number for pagination, default 1
  - `pageSize` (int, optional): Number of items per page, default 20
- **Response**:
  - **Success**: 200 OK
  ```json
  {
    "totalCount": 0,
    "totalPages": 0,
    "currentPage": 0,
    "pageSize": 0,
    "items": [
      {
        "id": "uuid",
        "generatedAt": "datetime",
        "totalGenerated": 10,
        "totalAccepted": 8,
        "sourceTextLength": 1500
      }
    ]
  }
  ```
  - **Error**: 401 Unauthorized

## 3. Authentication and Authorization

### Authentication Mechanism
- **JWT (JSON Web Token) Authentication**
  - Tokens issued upon successful login
  - Tokens include user ID and roles
  - Expiration set to 24 hours
  - Refresh token mechanism for extended sessions
  - Tokens must be included in the Authorization header as Bearer tokens

### Authorization Rules
- **Row-Level Security**
  - Users can only access their own data
  - Implemented using ASP.NET Core Identity and custom authorization policies
  - All endpoints except registration and login require authentication
  - User ID from token used to filter data in database queries

### Security Considerations
- **HTTPS Required** for all API endpoints
- **CORS Policy** configured to allow only the frontend application
- **Rate Limiting** applied to prevent abuse:
  - Login/Register: 5 requests per minute
  - Flashcard generation: 10 requests per hour
  - General API endpoints: 60 requests per minute
- **Input Validation** using FluentValidation for all requests
- **Password Requirements**:
  - Minimum 8 characters
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one digit
  - At least one special character

## 4. Validation and Business Logic

### User Validation
- Username: 3-50 characters, alphanumeric and underscores only
- Email: Valid email format, maximum 256 characters
- Password: Meets security requirements listed above

### Flashcard Validation
- Front: Required, 1-300 characters
- Back: Required, 1-500 characters
- Performance Rating: Integer between 0 and 5

### Study Session Validation
- Cannot end a session that is already ended
- Cannot record reviews for ended sessions
- Performance rating must be between 0 and 5

### Flashcard Generation Validation
- Source text: Required, 1000-10000 characters
- Count: Required, between 1 and 20

### Business Logic Implementation
- **Spaced Repetition Algorithm**
  - Implemented in the API layer
  - Updates flashcard metadata (EaseFactor, Interval, NextReviewDate) based on performance rating
  - SM-2 algorithm used as the basis for calculations
  
- **Flashcard Generation**
  - Text sent to AI service for processing
  - Results filtered for quality and relevance
  - Statistics recorded for generation and acceptance rates
  
- **Study Session Management**
  - Sessions automatically track number of flashcards reviewed
  - Session details store performance data for analytics
  - Flashcard metadata updated after each review

## 5. Additional Considerations

### Pagination
- All list endpoints support pagination
- Default page size: 20 items
- Maximum page size: 100 items

### Error Handling
- Standardized error response format:
```json
{
  "status": 400,
  "title": "Bad Request",
  "detail": "The request contains invalid parameters",
  "errors": {
    "propertyName": ["error message"]
  }
}
```

### Logging
- Request/response logging for debugging
- Performance metrics for monitoring
- Error logging for troubleshooting
- Audit logging for security events

### API Versioning
- API versioning through URL path: `/api/v1/resource`
- Initial version: v1
- Version included in Swagger documentation

### Documentation
- Swagger/OpenAPI documentation available at `/swagger`
- API documentation includes examples and descriptions
- Authentication flow documented for developers 