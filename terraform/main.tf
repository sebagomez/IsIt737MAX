
resource "azurerm_resource_group" "isit737max_resource_group" {
  name     = "${local.function_name}-rg"
  location = var.location
}

# https://www.maxivanov.io/deploy-azure-functions-with-terraform/
resource "azurerm_app_service_plan" "isit737max_service_plan" {
  name                = "${local.function_name}-sp"
  location            = azurerm_resource_group.isit737max_resource_group.location
  resource_group_name = azurerm_resource_group.isit737max_resource_group.name
  kind                = local.service_plan_kind
  reserved            = local.service_plan_reserved

  sku {
    tier = var.sp_sku_tier
    size = var.sp_sku_size
  }
}

resource "azurerm_application_insights" "isit737max_application_insights" {
  name                = "${local.function_name}-ai"
  location            = azurerm_resource_group.isit737max_resource_group.location
  resource_group_name = azurerm_resource_group.isit737max_resource_group.name
  application_type    = "Node.JS"
}

resource "azurerm_function_app" "isit737max_function_app" {
  name                       = local.function_name
  location                   = azurerm_resource_group.isit737max_resource_group.location
  resource_group_name        = azurerm_resource_group.isit737max_resource_group.name
  app_service_plan_id        = azurerm_app_service_plan.isit737max_service_plan.id
  storage_account_name       = var.function_storage_account
  storage_account_access_key = var.function_storage_key
  os_type                    = "linux"
  version                    = "~3"

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"       = 1 # Needs to be 1 so the GitHub Action can deploy the package
    "FUNCTIONS_WORKER_RUNTIME"       = local.runtime
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.isit737max_application_insights.instrumentation_key,
  }

  site_config {
    dotnet_framework_version = local.runtime_version
  }
}

output "function_output" {
  value = azurerm_function_app.isit737max_function_app
}