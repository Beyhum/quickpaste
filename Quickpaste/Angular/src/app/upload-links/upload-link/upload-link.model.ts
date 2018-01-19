/**
 * Represents an upload link. A paste can be created using an upload link's quick_link field
 * 
 * @export
 * @interface UploadLinkModel
 */
export interface UploadLinkModel {
    quick_link: string;
    allow_file: boolean;
}