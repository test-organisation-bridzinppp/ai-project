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

resource "azurerm_key_vault" "ai-vault" {
  depends_on = [ azurerm_kubernetes_cluster.ai-rg ]
  name                        = "keyvault-ai"
  location                    = azurerm_resource_group.ai-rg.location
  resource_group_name         = azurerm_resource_group.ai-rg.name
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name                    = "standard"
  soft_delete_retention_days  = 1
  purge_protection_enabled    = false
  public_network_access_enabled = true

  access_policy = [
    {
      tenant_id = data.azurerm_client_config.current.tenant_id
      object_id = data.azurerm_client_config.current.object_id

      secret_permissions = [
        "Set",
        "Get",
        "List",
        "Delete",
      ]
    },
    {
      application_id = ""
      certificate_permissions = []
      tenant_id = data.azurerm_client_config.current.tenant_id
      object_id = data.azurerm_kubernetes_cluster.ai-aks.key_vault_secrets_provider[0].secret_identity[0].object_id
      key_permissions = [
        "Get",
      ]
      secret_permissions = [
        "Get",
      ]
      storage_permissions = [
        "Get",
      ]
    }]
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
  key_vault_id = azurerm_key_vault.ai-vault.id  
}

module "ai-document" {
  depends_on = [ azurerm_resource_group.ai-rg ]
  source = "./modules/ai-document"
  resource_group_name = var.rg-name
  location = var.location  
}

