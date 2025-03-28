resource "azurerm_cognitive_account" "vision-ai-service" {  
  name                = "vision-ai-service"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku_name            = "F0"
  kind                = "ComputerVision" # other kinds: ComputerVision, Face, Emotion, FromRecognizer, TextAnalytics etc.
}
