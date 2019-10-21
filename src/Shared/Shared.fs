namespace Shared

open System

type Product =
    { ProductId: Guid
      InventoryId: Guid
      Name: string
      Description: string option
      TaxCodeId: Guid
      TaxProviderId: string
      MerchRank: int option
      PickingLocation: string option
      ShowNutrition: bool
      VariantId: Guid
      BaseUnitMultiplier: decimal option
      Sku: string
      ContainerSize: decimal option
      OrderBy: int
      Stock: int
      RealUnit: string option
      RealUnitFormatted: string option
      RealUnitAmount: decimal option
      RealUnitIsApproximate: bool option
      PackagingUnitAmount: decimal option
      PackageUnitAmount: decimal option
      PackageUnit: string option
      PackageUnitFormatted: string option
      Price: decimal
      SalePrice: decimal option
      SaleMinQuantity: int option
      ScalePrice: decimal option
      ScaleMinQuantity: int option
      MaxQuantity: int option
      RetailPrice: decimal option
      SaleStart: DateTime option
      SaleEnd: DateTime option
      FromLocation: string option
      ImperfectReason: string option
      ProductName: string
      VariantName: string option
      ProductDescription: string option
      ImageFilename: string
      HasNutritionInfo: bool
      Tags: Tag[]
      Categories: Category[]
      IsVisible: bool }
and Tag =
    { TagId: Guid
      Name: string
      CommonId: string }
and Category =
    { CategoryId: Guid
      Name: string
      CommonId: string
      ParentId: Guid option
      ThumbnailUrl: string option }