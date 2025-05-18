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


data "azurerm_client_config" "current" {}


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
  source = "./modules/aks"
  resource_group_name = var.rg-name
  location = var.location  
}

data "azurerm_kubernetes_cluster" "ai-aks" {
  depends_on = [ module.aks ]
  name                = "ai-aks-cluster"
  resource_group_name = var.rg-name
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

data "azurerm_search_service" "ai-search" {  
  depends_on = [ module.ai-search ]
  name                = "ai-search-pb1980"
  resource_group_name = var.rg-name
}

module "ai-document" {
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/ai-document"
  resource_group_name = var.rg-name
  location = var.location  
}