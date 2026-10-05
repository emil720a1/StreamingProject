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
      id: 'stream-1',
      userId: 'user-1',
      streamerUsername: 'Alex',
      title: 'Gaming Live',
      description: 'Gaming stream',
      startTime: '2026-10-05T18:00:00Z',
    });
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
