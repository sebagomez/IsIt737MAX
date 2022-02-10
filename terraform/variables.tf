variable "location" {
  type        = string
  default     = "East US"
  description = "Default location where the Azure Function will be created"
}

variable "function_storage_account" {
  type        = string
  default     = ""
  description = "Storage account used to store the function"
}

variable "function_storage_key" {
  type        = string
  default     = ""
  description = "Storage key used to store the function"
}

variable "sp_sku_tier" {
  type        = string
  default     = "Dynamic"
  description = "SKU tier for the function's Service Plan"
}

variable "sp_sku_size" {
  type        = string
  default     = "Y1"
  description = "SKU size for the function's Service Plan"
}

locals {
  function_name         = "isit737max"
  runtime               = "dotnet"
  runtime_version       = "v4.0"
  service_plan_kind     = "FunctionApp"
  service_plan_reserved = true
  tags = {
    "owner" : "sebagomez",
    "platform" : "dotnet core",
    "version" : "3.1"
  }
}