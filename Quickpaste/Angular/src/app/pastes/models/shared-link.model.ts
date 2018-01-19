/**
 * Represents the required body fields to create a shared link
 * 
 * @export
 * @interface SharedLinkCreateModel
 */
export interface SharedLinkCreateModel {
    duration_in_minutes: number;
}

/**
 * Represents an existing shared link
 * 
 * @export
 * @interface SharedLinkModel
 */
export interface SharedLinkModel {
    duration_in_minutes: number;
    blob_url: string;
}