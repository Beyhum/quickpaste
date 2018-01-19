/**
 * Represents an existing paste
 * 
 * @export
 * @interface PasteModel
 */
export interface PasteModel {
    id: string;
    blob_url: string;
    message: string;
    quick_link: string;
    is_public: boolean;
    created_at: Date;
}

/**
 * Represents the body fields to create a paste as an authenticated user
 * 
 * @export
 * @interface PasteCreateModel
 */
export interface PasteCreateModel {
    quick_link: string;
    message: string
    is_public: boolean;
    file_to_upload: Blob

}

/**
 * Represents the body fields to create a paste through an upload link (i.e. created without authentication/identity)
 * 
 * @export
 * @interface AnonPasteCreateModel
 */
export interface AnonPasteCreateModel {
    message: string
    file_to_upload: Blob

}