import {Component, OnInit} from '@angular/core';
import {
  StudentGetAllEndpointService, StudentGetAllRequest,
  StudentGetAllResponse
} from '../../../../endpoints/student-endpoints/student-get-all-endpoint.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {MyPagedList} from '../../../../helper/my-paged-list';
import {MyPagedRequest} from '../../../../helper/my-paged-request';
import {MatTableDataSource} from '@angular/material/table';
import {FormControl} from '@angular/forms';
import {PageEvent} from '@angular/material/paginator';
import {debounceTime} from 'rxjs';
import {MyDialogConfirmComponent} from '../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import {MatDialog} from '@angular/material/dialog';
import {MyConfig} from '../../../../my-config';

@Component({
  selector: 'app-student-list',
  standalone: false,

  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})
export class StudentListComponent implements  OnInit {
  students: MyPagedList<StudentGetAllResponse>| null=null;
  pageRequest:StudentGetAllRequest={
    pageNumber:1,
    pageSize:100
  }
  dataSource=new MatTableDataSource<StudentGetAllResponse>();
  displayedColumns: string[] = ['firstName', 'lastName', 'studentNumber', 'actions'];
  SearcName= new FormControl('');
  showDeleted=true;

  deletedCss={
    'text-decoration': 'line-through',
  'color': 'gray'

}
  constructor(private router: Router,
              private route: ActivatedRoute,
              private http: HttpClient,
              private allStudents: StudentGetAllEndpointService,
              private dialog: MatDialog) {
  }
  ngOnInit() {
    this.loadStudents();

    this.SearcName.valueChanges.pipe(debounceTime(300)).
    subscribe({
      next: data => {
        this.pageRequest.firstLast=data ?? undefined;
        this.loadStudents();
      }
    })
  }

  loadStudents(){
    this.allStudents.handleAsync(this.pageRequest, true).subscribe({
      next: data =>{
        this.students=data;
        this.dataSource.data=data.dataItems;
      }
    })
  }

  updateStudent(id:number) {
    this.router.navigate(['admin/students/update/',id]);
  }

  openMyConfirmDialog(id: number) {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '350px',
      data: {
        title: 'Potvrda brisanja',
        message: 'Da li ste sigurni da želite obrisati ovu stavku?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Korisnik je potvrdio brisanje');
        // Pozovite servis ili izvršite logiku za brisanje
        this.deleteStudent(id);
      } else {
        console.log('Korisnik je otkazao brisanje');
      }
    });
  }

  pageStudents(page: PageEvent) {
    this.pageRequest.pageNumber=page.pageIndex+1;
    this.pageRequest.pageSize=page.pageSize;
    this.loadStudents();
  }

  private deleteStudent(id: number) {
    this.http.delete<any>(`${MyConfig.api_address}/students/delete/${id}`).subscribe({
      next: data => {
        let deleted=this.students?.dataItems.find(s=>s.id==id);
        if(deleted){
          deleted.isDeleted=true;
        }
      }
    })
  }

  filterStd(){
    this.showDeleted=!this.showDeleted;

    if(this.showDeleted)
      {
        this.dataSource.data=this.students!.dataItems;
      }
    else{
      this.dataSource.data=this.students!.dataItems.filter(val=>!val.isDeleted);
    }
  }

  openSemester(id:number) {
    this.router.navigate(['/admin/students/semesters/', id]);
  }
}

