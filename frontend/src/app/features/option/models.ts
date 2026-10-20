export interface OptionResponse {
    id: number;
    name: string;
    optionValues: OptionValueResponse[];
}

export interface OptionValueResponse {
    id: number;
    value: string;
}

export interface CreateOptionRequest {
    name: string;
    optionValues: string[]
}

export interface UpdateOptionRequest {
    name: string;
    optionValues: UpdateOptionValueRequest[]
}

export interface UpdateOptionValueRequest {
    id?: number;
    value: string;
}