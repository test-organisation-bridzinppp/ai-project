resource "azurerm_cognitive_account" "text-ai-service" {  
  name                = "text-ai-service"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku_name            = "F0"
  kind                = "TextAnalytics" # other kinds: ComputerVision, Face, Emotion, FromRecognizer, TextAnalytics etc.
}
