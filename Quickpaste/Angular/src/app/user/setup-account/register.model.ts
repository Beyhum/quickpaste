export class RegisterModel {
    username: string;
    password: string;
    default_username: string;
    default_passcode: string;

    constructor(default_username: string, default_passcode: string, username: string, password: string){
        this.default_username = default_username;
        this.default_passcode = default_passcode;
        this.username = username;
        this.password = password;
    }
}

export class RegisterResponse {
    success: boolean;    
    display_text: string;

    constructor(success: boolean, displayText: string){
        this.success = success;
        this.display_text = displayText;
    }
}