import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styles: [
    `
    h1 {
      margin-top: 0px;
    }
    p {
      font-size: 1.2em;
    }
    .control-label {
      margin-top: 30px;
    }
    #footnote {
      margin: 5px 0px;
    }
    `]
})
export class WelcomeComponent implements OnInit {

  uploadLink: string = null;
  pasteQuickLink: string = null;

  constructor(private router: Router) { }

  onUploadLinkChanged(keyUpEvent: any) {
    this.uploadLink = keyUpEvent.target.value;

  }

  onpasteQuickLinkChanged(keyUpEvent: any) {
    this.pasteQuickLink = keyUpEvent.target.value;
  }

  goToPaste() {
    this.router.navigate(['/pastes', this.pasteQuickLink]);
  }

  goToUploadLink() {
    this.router.navigate(['/u', this.uploadLink]);

  }


  ngOnInit() {
  }

}
