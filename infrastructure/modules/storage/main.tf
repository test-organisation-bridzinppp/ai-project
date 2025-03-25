resource "azurerm_storage_account" "ai-storage" {  
  name                     = "aistoragepb1980"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags = {
    Environment = "Production",
    Content     = "AI data"
  }
}

resource "azurerm_storage_share" "ai-file-share" {
  name                 = "aifilesharepb1980"
  storage_account_id   = azurerm_storage_account.ai-storage.id  
  quota                = 50
}
