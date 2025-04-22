# SendAuditToDataverse Azure Function

This Azure Function is designed to receive audit data via a POST request and send it to a Microsoft Dataverse instance. The function processes the incoming audit records, extracts relevant information, and inserts it into the Dataverse table `cr356_customauditlogs`.

## Function Overview

The `SendAuditToDataverse` Azure Function listens for HTTP POST requests and processes the audit data in the request body. The function then inserts each audit record into a custom table `cr356_customauditlogs` in Microsoft Dataverse.

### Core Features:
- Receives audit data in JSON format via an HTTP POST request.
- Extracts audit details such as `auditid`, `createdon`, `userid`, `objecttypecode`, `objectid`, and `changedata`.
- Inserts each audit record into the `cr356_customauditlogs` table in Dataverse.
- Handles errors gracefully and logs them for troubleshooting.

## Requirements

- **Azure Functions** runtime (v4 or above)
- **Microsoft Power Platform Dataverse Client** library for Dataverse integration
- A valid **Dataverse Connection String** (configured as an environment variable `DataverseConnectionString`)

## Dataverse Table Schema

This function inserts data into the `cr356_customauditlogs` table with the following fields:

- `cr356_auditid`: Unique identifier of the audit record.
- `cr356_createdon`: Date and time when the audit was created.
- `cr356__userid_value`: User ID associated with the audit.
- `cr356_objecttypecode`: Type code of the object being audited.
- `cr356__objectid_value`: Object ID that was audited.
- `cr356_changedata`: Data that reflects the changes made.

## Environment Setup

To run this function, ensure the following environment settings are configured:

1. **Dataverse Connection String**:  
   Set up the connection string for Dataverse as an environment variable named `DataverseConnectionString` in the Azure Function app settings or in your local settings file (local.settings.json) for local development.

## Function Trigger

This function is triggered by an HTTP POST request. The request body should contain the audit data in the following structure:

```json
{
  "value": [
    {
      "auditid": "string",
      "createdon": "string",
      "_userid_value": "string",
      "objecttypecode": "string",
      "_objectid_value": "string",
      "changedata": "string"
    }
  ]
}
```

- **auditid**: The unique ID of the audit record.
- **createdon**: The timestamp of when the audit was created.
- **_userid_value**: The user ID who performed the action.
- **objecttypecode**: The type of the object being audited.
- **_objectid_value**: The unique identifier of the object.
- **changedata**: A string that contains the details of the change made (optional).

## Error Handling

The function handles errors and will log any issues encountered during the processing of the audit data. If an error occurs during the insertion process, the HTTP response will return a status code of 500 (Internal Server Error) along with the error message.

## Usage Example

You can test this Azure Function by sending a POST request to the function URL with a JSON body containing the audit data.

**Example HTTP Request:**

```bash
POST https://<your-function-url>/api/SendAuditToDataverse
Content-Type: application/json

{
  "value": [
    {
      "auditid": "12345",
      "createdon": "2025-04-22T10:00:00Z",
      "_userid_value": "user-guid",
      "objecttypecode": "account",
      "_objectid_value": "account-guid",
      "changedata": "{\"changedAttributes\":[{\"logicalName\":\"name\",\"oldValue\":\"Old Account\",\"newValue\":\"New Account\"}]}"
    }
  ]
}
```

### Response Example:
On successful insertion, the function will respond with:

```json
{
  "statusCode": 200,
  "message": "Inserted 1 audit records."
}
```

If an error occurs, it will return:

```json
{
  "statusCode": 500,
  "message": "Error: [error-message]"
}
```

## Contributing

If you find any issues or would like to improve the functionality, feel free to open a pull request or submit an issue.
