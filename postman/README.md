# Postman collection

Import `task-management-api.postman_collection.json` into Postman Desktop or the Postman web app. Select the `Task Management Local` environment from `environments/task-management-local.environment.yaml` if using the Git-native collection files.

The collection runs the task workflow in order:

1. Create task (`POST /api/tasks`)
2. List filtered tasks (`GET /api/tasks`)
3. Get task by id (`GET /api/tasks/{id}`)
4. Update task (`PUT /api/tasks/{id}`)
5. Delete task (`DELETE /api/tasks/{id}`)

The create test stores the generated task ID in the active environment for the remaining requests.

Run locally with Postman CLI:

```powershell
postman collection lint .\postman\collections\task-management-api
postman environment lint .\postman\environments\task-management-local.environment.yaml
postman collection run .\postman\collections\task-management-api -e .\postman\environments\task-management-local.environment.yaml --no-report-events
```
