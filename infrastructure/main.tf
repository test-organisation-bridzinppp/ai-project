terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.24.0"
    }
  }
}

provider "azurerm" {
  subscription_id = "ccddcc4d-ca09-4673-ab49-572f8d9e98ea"
  features {}  
}

resource "azurerm_resource_group" "ai-rg" {
  name     = "ai-rg"
  location = "East US"
}