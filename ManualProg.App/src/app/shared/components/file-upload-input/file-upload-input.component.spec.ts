import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FileUploadInputCompontent } from './file-upload-input.component';

describe('FileUploadInputCompontent', () => {
  let component: FileUploadInputCompontent;
  let fixture: ComponentFixture<FileUploadInputCompontent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FileUploadInputCompontent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FileUploadInputCompontent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
