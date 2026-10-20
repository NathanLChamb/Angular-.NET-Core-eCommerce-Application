export interface CartResponse {
  id: number;
  createdAt: string;
  updatedAt: string | null;
  items: CartItemResponse[];
  subtotal: number;
}

export interface CartItemResponse {
  id: number;
  productVariantId: number;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
  imageUrl: string | null;
}

export interface AddItemRequest {
  productVariantId: number;
  quantity: number;
}

export interface UpdateItemRequest {
  quantity: number;
}

export interface ReadCartItemDto {
  id: number;
  productVariantId: number;
  productName: string;
  optionValues: ReadOptionValueFromCartDto[];
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

export interface ReadOptionValueFromCartDto {
  value: string;
}

export interface AddCartItemDto {
  productVariantId: number;
  quantity: number;
}

export interface UpdateCartItemDto {
  quantity: number;
}