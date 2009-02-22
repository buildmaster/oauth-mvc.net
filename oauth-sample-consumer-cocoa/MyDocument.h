//
//  MyDocument.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright Xero.com 2009 . All rights reserved.
//


#import <Cocoa/Cocoa.h>
#import "ManagingViewController.h"
#import "ViewParent.h"
@interface MyDocument : NSPersistentDocument<ViewParent>
{
	IBOutlet NSBox *box;
	NSMutableArray *viewControllers;
	NSMutableDictionary *sharedValueDictionary;
}
- (IBAction) changeViewController:(id)sender;
-(void)displayViewController:(ManagingViewController *)vc;

@end
