resource "azurerm_cognitive_account" "open_ai" {  
  name                = "openai"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku_name            = "S0"
  kind                = "OpenAI"
}

resource "azurerm_cognitive_deployment" "open_ai_deployment" {
  name                 = "openai-deployment"
  cognitive_account_id = azurerm_cognitive_account.open_ai.id
  sku {
    name = "Standard"
  }
  model {
    format  = "OpenAI"
    name    = "gpt-4"
    version = "turbo-2024-04-09"
  }  
}
