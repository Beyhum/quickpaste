import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PasteModel } from '../models/paste.model';
import { Location } from '@angular/common';
import { PasteService } from '../paste.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../auth/auth.service';

@Component({
  templateUrl: './paste-detail.component.html',
  styleUrls: ['./paste-detail.component.css']
})
export class PasteDetailComponent implements OnInit {

  errorMessage: string = null;
  paste: PasteModel;

  authService: AuthService;

  constructor(private pasteService: PasteService, private toastr: ToastrService, private route: ActivatedRoute,
    private router: Router, authService: AuthService, private location: Location) {
    this.authService = authService;
  }

  deletePaste() {
    if (window.confirm("Are you sure you want to delete this paste?")) {
      this.pasteService.deletePaste(this.paste.id).subscribe((success: boolean) => {
        if (success) {
          this.toastr.success("Paste deleted");
          this.router.navigate(['/pastes']);
        } else {
          this.toastr.error("Failed to delete paste");
        }
      });
    }

  }

  goBack() {
    this.location.back();
  }


  ngOnInit() {
    this.route.data.forEach((data) => {
      this.paste = data['paste'][0];
      this.errorMessage = data['paste'][1];
    });
  }

}
