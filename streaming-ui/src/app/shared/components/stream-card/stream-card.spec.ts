import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { StreamCard } from './stream-card';

describe('StreamCard', () => {
  let component: StreamCard;
  let fixture: ComponentFixture<StreamCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StreamCard],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(StreamCard);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('stream', {
      id: 1,
      title: 'Gaming Live',
      author: 'Alex',
      viewers: 1240,
      category: 'Gaming',
      isLive: true,
    });
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
