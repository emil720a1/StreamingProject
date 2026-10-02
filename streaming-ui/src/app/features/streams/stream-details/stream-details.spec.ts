import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { StreamDetails } from './stream-details';

describe('StreamDetails', () => {
  let component: StreamDetails;
  let fixture: ComponentFixture<StreamDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StreamDetails],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(StreamDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
