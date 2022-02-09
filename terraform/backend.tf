
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.91.0"
    }
  }

  backend "azurerm" {
    # Provided by the pipeline via secrets
    # resource_group_name  = var.backend_res_group
    # storage_account_name = var.backend_storage_account
    # container_name       = var.backend_storage_container
    key = "terraform.tfstate"
  }
}

# https://jeffbrown.tech/terraform-azure-authentication/
provider "azurerm" {
  features {}
}

# this env vars must be set
# export ARM_ACCESS_KEY=

# export ARM_SUBSCRIPTION_ID=
# export ARM_TENANT_ID=
# export ARM_CLIENT_ID=
# export ARM_CLIENT_SECRET=