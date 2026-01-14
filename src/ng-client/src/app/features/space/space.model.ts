export interface SpaceDto {
    id: string;
    name: string;
    description?: string;
    createdBy: string;
    createdAt: Date;
    updatedAt: Date;
    storageId: string;
    storagePath: string;
}

export interface CreateSpaceRequest {
    name: string;
    description?: string;
    storageId: string;
    storagePath: string;
}

export interface UpdateSpaceRequest {
    name: string;
    description?: string;
}