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
  name     = var.rg-name
  location = var.location
}

module "storage" {
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/storage"
  resource_group_name = var.rg-name
  location = var.location  
}

module "aks" {
  depends_on = [ module.storage ]
  source = "./modules/aks"
  resource_group_name = var.rg-name
  location = var.location  
}

module "ai-openai" {
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/ai-openai"
  resource_group_name = var.rg-name
  location = var.location  
}

module "ai-text" {
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/ai-text"
  resource_group_name = var.rg-name
  location = var.location  
}

module "ai-vision" {
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/ai-vision"
  resource_group_name = var.rg-name
  location = var.location  
}

module "ai-search" {
  count = var.enable_search ? 1 : 0
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/ai-search"
  resource_group_name = var.rg-name
  location = var.location  
}