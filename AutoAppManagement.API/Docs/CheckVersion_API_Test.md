# Test CheckVersion API với VersionResponse

## 1. Test CheckVersion API
```bash
POST /api/ToolVersion/check
Content-Type: application/json

{
  "toolCode": "MyTool",
  "currentVersion": "1.0.0",
  "platform": "Windows"
}
```

## Expected Response:
```json
{
  "success": true,
  "message": "",
  "data": {
    "updateAvailable": true,
    "updateRequired": false,
    "latestVersion": {
      "version": "1.1.0",
      "downloadUrl": "https://example.com/download/v1.1.0/mytool.exe",
      "changelogUrl": "Release notes content here...",
      "description": "Description of the tool",
      "checksum": "sha256:abcd1234...",
      "mandatory": false,
      "fileSize": 52428800
    },
    "message": "A new version is available."
  }
}
```

## 2. Khi không có update:
```json
{
  "success": true,
  "message": "",
  "data": {
    "updateAvailable": false,
    "updateRequired": false,
    "latestVersion": null,
    "message": "You are using the latest version."
  }
}
```

## 3. Khi có update bắt buộc:
```json
{
  "success": true,
  "message": "",
  "data": {
    "updateAvailable": true,
    "updateRequired": true,
    "latestVersion": {
      "version": "2.0.0",
      "downloadUrl": "https://example.com/download/v2.0.0/mytool.exe",
      "changelogUrl": "Major version update...",
      "description": "Major version with breaking changes",
      "checksum": "sha256:efgh5678...",
      "mandatory": true,
      "fileSize": 67108864
    },
    "message": "A required update is available. Please update immediately."
  }
}
```

## Sử dụng trong client code:
```javascript
const response = await fetch('/api/ToolVersion/check', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    toolCode: 'MyTool',
    currentVersion: '1.0.0',
    platform: 'Windows'
  })
});

const result = await response.json();
if (result.data.updateAvailable) {
  const latestVersion = result.data.latestVersion;
  console.log('New version:', latestVersion.version);
  console.log('Download URL:', latestVersion.downloadUrl);
  console.log('File size:', latestVersion.fileSize);
  console.log('Is mandatory:', latestVersion.mandatory);
}
```