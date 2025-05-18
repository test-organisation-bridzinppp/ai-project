resource "azurerm_search_service" "ai-search" {  
  name                = "ai-search-pb1980"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "free"
  local_authentication_enabled = false
  authentication_failure_mode  = "http403"
}