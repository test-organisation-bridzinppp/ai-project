variable "location" {
  description = "The Azure Region in which all resources will be created."
  type = string  
}

variable "rg-name" {
  description = "The name of the Azure Resource Group."
  type = string  
}

variable "enable_search" {
  description = "Enable or disable the AI Search module."
  type = bool
  default = false
  validation {
    condition = var.enable_search == true || var.enable_search == false
    error_message = "enable_search must be a boolean value."
  }  
}