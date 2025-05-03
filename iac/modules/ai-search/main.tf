resource "azurerm_search_service" "ai-search" {  
  name                = "ai-search-pb1980"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "free"
  local_authentication_enabled = true
  authentication_failure_mode  = "http403"
}

data "azurerm_search_service" "this" {
  name                = azurerm_search_service.ai-search.name
  resource_group_name = var.resource_group_name
}

resource "azurerm_key_vault_secret" "query_key" {
  name         = "ai-search-key"
  value        = data.azurerm_search_service.this.primary_key
  key_vault_id = var.key_vault_id
}
