export enum UserStorageType {
    FileSystem,
    Smb,
    Dropbox,
    GoogleDrive,
    GooglePhotos,
    GoogleCloudStorage,
    OneDrive,
    AwsS3,
    AzureBlob
}

export interface CreateStorageRequest {
    type: UserStorageType;
    name: string;
    description?: string;
    credentialsJson: any;
}

export interface UpdateStorageRequest {
    name: string;
    description?: string;
    credentialsJson: any;
}

export interface StorageDto {
    id: string;
    type: UserStorageType;
    name: string;
    description?: string;
    createdBy: string;
    createdAt: Date;
    updatedAt: Date;
    error?: string;
    quotaBytes: number;
    usedBytes: number;
}