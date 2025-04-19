resource "azurerm_cognitive_account" "document-ai-service" {  
  name                = "document-ai-service"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku_name            = "S0"
  kind                = "FormRecognizer" # other kinds: ComputerVision, Face, Emotion, FromRecognizer, TextAnalytics etc.
}
