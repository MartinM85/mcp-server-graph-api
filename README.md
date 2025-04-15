# MCP Server for the Microsoft Graph Api
Model-context-protocol (MCP) server for the Microsoft Graph API in C#.

## Prerequisites

### Entra ID application

Register a new Entra ID application, add and grant at least the `User.Read.All` application permission and create a new client secret.

You can grant also another permissions, it depends on what you want to ask AI about your tenant through the Microsoft Graph API.

### Claude Desktop app

I'm using the [Claude Desktop app](https://claude.ai/download), but I think you can use any other MCP clients, including GitHub Copilot if they allow to add local MCP server.

## MCP server

Open and build the solution.

### MCP server configuration

Open the `claude_desktop_config.json` file. On Windows, the file should be located in **\%APPDATA%\Claude\\** folder.

Add the MCP Server. Under the `args` section, specify the path to project folder. Don't forget to set the details about the Entra ID application like tenant id, client id and client secret under the `env` section.

By default, Microsoft Graph global service is used. You can change it by modifying the `NATIONAL_CLOUD` environment variable.

Possible values for the `NATIONAL_CLOUD` are:

- **Global**: https://graph.microsoft.com - Microsoft Graph global service
- **US_GOV**: https://graph.microsoft.us - Microsoft Graph for US Government L4
- **US_GOV_DOD**: https://dod-graph.microsoft.us - Microsoft Graph for US Government L5 (DOD)
- **China**: https://microsoftgraph.chinacloudapi.cn - Microsoft Graph China operated by 21Vianet
- **Germany**: https://graph.microsoft.de - Microsoft Graph for Germany

```
{
    "mcpServers": {
        "graphApi": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "path/to/folder/with/console_project",
                "--no-build"
            ],
            "env": {
                "TENANT_ID": "<tenant_id>",
                "CLIENT_ID": "<client_id>",
                "CLIENT_SECRET": "<client_secret>",
                "NATIONAL_CLOUD": "Global"
            }
        }
    }
}
```

## Run MCP server

When you open Claude Desktop app, the MCP server should be automatically discovered and run. You can start new chat.

If Claude Desktop app was already open, you need to close (quit it in system tray) and reopen it.

In case of any issues, you can check logs in **\%APPDATA%\Claude\\logs**.


