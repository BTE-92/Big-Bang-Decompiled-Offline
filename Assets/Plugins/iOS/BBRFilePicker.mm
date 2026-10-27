// Import level on iPhone / iPad: shows Apple's Files browser (UIDocumentPickerViewController) on top of the game. The chosen file is
// copied into the folder the game gives (picked.zip + picked.name) and picked.done is written ("ok", "cancel" or "error"): the same
// files the Android picker (PickerActivity) writes, so the game reads both the same way.
#import <UIKit/UIKit.h>

extern "C" UIViewController *UnityGetGLViewController();

static void BBRWriteText(NSString *dir, NSString *name, NSString *text)
{
    [[NSFileManager defaultManager] createDirectoryAtPath:dir withIntermediateDirectories:YES attributes:nil error:nil];
    [text writeToFile:[dir stringByAppendingPathComponent:name] atomically:YES encoding:NSUTF8StringEncoding error:nil];
}

@interface BBRPickerDelegate : NSObject <UIDocumentPickerDelegate>
@property (nonatomic, copy) NSString *dir;
@end

@implementation BBRPickerDelegate

- (void)importURL:(NSURL *)url
{
    NSFileManager *fm = [NSFileManager defaultManager];
    [fm createDirectoryAtPath:self.dir withIntermediateDirectories:YES attributes:nil error:nil];
    NSString *tmp = [self.dir stringByAppendingPathComponent:@"picked.tmp"];
    NSString *zip = [self.dir stringByAppendingPathComponent:@"picked.zip"];
    [fm removeItemAtPath:tmp error:nil];
    [fm removeItemAtPath:zip error:nil];
    BOOL scoped = [url startAccessingSecurityScopedResource];
    NSError *error = nil;
    BOOL copied = [fm copyItemAtURL:url toURL:[NSURL fileURLWithPath:tmp] error:&error];
    if (scoped)
    {
        [url stopAccessingSecurityScopedResource];
    }
    if (copied && [fm moveItemAtPath:tmp toPath:zip error:nil])
    {
        BBRWriteText(self.dir, @"picked.name", [url lastPathComponent]);
        BBRWriteText(self.dir, @"picked.done", @"ok");
    }
    else
    {
        BBRWriteText(self.dir, @"picked.done", @"error");
    }
}

// iOS 8 - 10
- (void)documentPicker:(UIDocumentPickerViewController *)controller didPickDocumentAtURL:(NSURL *)url
{
    [self importURL:url];
}

// iOS 11 and later
- (void)documentPicker:(UIDocumentPickerViewController *)controller didPickDocumentsAtURLs:(NSArray<NSURL *> *)urls
{
    if ([urls count] > 0)
    {
        [self importURL:[urls objectAtIndex:0]];
    }
    else
    {
        BBRWriteText(self.dir, @"picked.done", @"cancel");
    }
}

- (void)documentPickerWasCancelled:(UIDocumentPickerViewController *)controller
{
    BBRWriteText(self.dir, @"picked.done", @"cancel");
}

@end

static BBRPickerDelegate *s_pickerDelegate = nil;

extern "C" int BBR_IOS_PickFile(const char *dirUtf8)
{
    if (dirUtf8 == NULL)
    {
        return 0;
    }
    NSString *dir = [NSString stringWithUTF8String:dirUtf8];
    dispatch_async(dispatch_get_main_queue(), ^{
        UIViewController *root = UnityGetGLViewController();
        if (root == nil)
        {
            BBRWriteText(dir, @"picked.done", @"error");
            return;
        }
        s_pickerDelegate = [[BBRPickerDelegate alloc] init];
        s_pickerDelegate.dir = dir;
        UIDocumentPickerViewController *picker = [[UIDocumentPickerViewController alloc] initWithDocumentTypes:@[ @"public.data" ] inMode:UIDocumentPickerModeImport];
        picker.delegate = s_pickerDelegate;
        [root presentViewController:picker animated:YES completion:nil];
    });
    return 1;
}