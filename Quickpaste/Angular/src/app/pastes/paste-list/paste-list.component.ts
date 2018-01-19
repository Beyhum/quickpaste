import { Component, OnInit, ElementRef } from '@angular/core';
import { PasteService } from '../paste.service';
import { PasteModel } from '../models/paste.model';
import { SharedLinkModel } from '../models/shared-link.model';
import { WindowRefService } from '../../shared/window-ref.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';


export const PAGE_BASE_LIMIT = 10;


@Component({
  selector: 'paste-list',
  templateUrl: './paste-list.component.html',
  styles: [`
  .option-buttons {
    margin-left: 5px;
    margin-bottom: 10px;
    text-align: right;
  }
  `]
})
export class PasteListComponent implements OnInit {

  pasteList: PasteModel[] = [];
  limit: number = 10;
  page: number = 1;
  nextExists: boolean = false;

  querySubscription: Subscription;

  constructor(private pasteService: PasteService, private windowRef: WindowRefService, private toastr: ToastrService, private route: ActivatedRoute, private router: Router) { }

  // simply changes the current route's query parameters. The querySubscription will pick up this change and automatically retrieve the desired page
  nextPage() {
    this.router.navigate(['pastes'], { queryParams: { page: this.page + 1 } });

  }

  previousPage() {
    this.router.navigate(['pastes'], { queryParams: { page: this.page -1 } });
  }

  private getPage() {
    this.pasteService.getPastes(this.page, this.limit).subscribe((pasteList: PasteModel[]) => {
      this.nextExists = pasteList.length >= this.limit;
      this.pasteList = pasteList;
    });
  }

  ngOnInit() {

    // subscribe to changes in the query parameters and get the according page of pastes
    this.querySubscription = this.route.queryParams.subscribe(params => {
        this.page = +params['page'] || 1; // get page 1 by default if page parameter is not specified
        this.getPage();
      });
  }

  ngOnDestroy() {
    this.querySubscription.unsubscribe();
  }

}
