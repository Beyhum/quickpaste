/**
 * Represents a JWT Token for authentication
 * 
 * @export
 * @class TokenModel
 */
export class TokenModel {
    access_token: string;
    expiration: string;
    username: string;
}