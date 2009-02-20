//
//  ManagingViewController.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "ViewParent.h"
extern NSString * const OACConsumerKey;
extern NSString * const OACConsumerSecret;
extern NSString * const OACRequestTokenKey;
extern NSString * const OACRequestTokenSecret;

@interface ManagingViewController : NSViewController {
	NSManagedObjectContext *managedObjectContext;
	id<ViewParent> parent;
}
@property (retain) NSManagedObjectContext *managedObjectContext;
@property (retain) id<ViewParent> parent;

@end


