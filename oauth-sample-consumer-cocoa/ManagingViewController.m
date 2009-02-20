//
//  ManagingViewController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "ManagingViewController.h"
NSString * const OACConsumerKey = @"ConsumerKey";
NSString * const OACConsumerSecret = @"ConsumerSecret";
NSString * const OACRequestTokenKey = @"RequestTokenKey";
NSString * const OACRequestTokenSecret = @"RequestTokenSecret";

@implementation ManagingViewController
@synthesize managedObjectContext;
@synthesize parent;
-(void) dealloc
{
	[managedObjectContext release];
	[super dealloc];	
}
@end
