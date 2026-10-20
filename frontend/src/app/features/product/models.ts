import { CategoryResponse } from "../category/models";
import { OptionResponse, OptionValueResponse } from "../option/models";

export interface ReadProductDto {
    id: number;
    name: string;
    description: string;
    createdAt: string;
    updatedAt: string;
    categories: CategoryResponse[];
    options: ReadOptionFromProductDto[];
    productVariants: ReadProductVariantDto[];
}

export interface ProductResponse {
    id: number;
    name: string;
    description: string;
    categories: CategoryResponse[];
    options: OptionResponse[];
    productVariants: ProductVariantResponse[];
}

export interface ProductVariantResponse {
    id: number;
    sku: string;
    price: number;
    stockQuantity: number;
    optionValues: OptionValueResponse[];
    pictures: PictureResponse[];
}

export interface PictureResponse {
  role: string;
  location: string;
}

export interface PictureRequest {
  role: string;
  location: string;
}

export interface CreateProductRequest {
    name: string;
    description: string;
    categoryIds: number[];
    optionIds: number[];
    productVariants: CreateProductVariantRequest[];
}

export interface CreateProductVariantRequest {
    sku: string;
    price: number;
    stockQuantity: number;
    optionValueIds: number[];
    pictures: PictureRequest[];
}

export interface UpdateProductRequest {
    name: string;
    description: string;
    categoryIds: number[];
    optionIds: number[];
    productVariants: UpdateProductVariantRequest[];
}

export interface UpdateProductVariantRequest {
    id? : number;
    sku: string;
    price: number;
    stockQuantity: number; 
    optionValueIds: number[];
    pictures: PictureRequest[];
}

export interface ReadOptionFromProductDto {
    id: number,
    name: string
}
export interface ReadProductVariantDto {
    id: number;
    sku: string;
    price: number;
    stockQuantity: number;
    createdAt: string;
    updatedAt: string;
    optionValues: ReadOptionValueFromProductVariantDto[];
    productVariantImages: PictureResponse[];
}

export interface ReadOptionValueFromProductVariantDto {
    id: number;
    value: string;
    optionId: number;
    optionName: string;
}

export interface CreateProductDto {
    name: string;
    description: string;
    categoryIds: number[];
    optionIds: number[];
    productVariants: CreateProductVariantDto[];
}

export interface CreateProductVariantDto {
    sku: string;
    price: number;
    stockQuantity: number;
    optionValueIds: number[];
}

export interface UpdateProductDto {
    name: string;
    description: string;
    categoryIds: number[];
    optionIds: number[];
    productVariants: UpdateProductVariantDto[]
}

export interface UpdateProductVariantDto {
    id?: number;
    sku: string;
    price: number;
    stockQuantity: number;
    optionValueIds: number[];
}

export interface ProductSearchFilter {
    search?: string;
    categoryIds: number[];
    optionIds: number[];
    sort: ProductSort;
    pageNumber: number;
    pageSize: number;
}

export enum ProductSort {
  Default = 0,
  PriceAscending = 1,
  PriceDescending = 2,
  Newest = 3
}