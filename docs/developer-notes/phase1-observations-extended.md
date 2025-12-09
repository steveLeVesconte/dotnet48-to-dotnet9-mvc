# Phase 1 Extended Developer Observations

**Purpose**: Detailed notes on edge cases, quirks, and "don't forget this" items 
that don't belong in formal baseline documentation but are worth tracking. After phase 4 these are candidates for improvement or items to check in phase 4

**Audience**: Future me 

---

## Shopping Cart - Browser Back Button Issue

**Discovered**: 2025-11-29 during smoke testing

**Reproduction**:
1. Add item to cart
2. Remove item via AJAX 
3. Click Checkout
4. Browser back button
5. Removed item still shows in DOM (stale cached page)
6. Click "Remove from cart" on phantom item → Exception

**Why This Happens**:
- AJAX removes item server-side
- jQuery updates DOM client-side
- Browser back = cached HTML before removal
- DOM shows item that doesn't exist in cart
- RemoveFromCart POST fails (item ID not found)

**Classification**: Edge case browser navigation issue, not normal user flow

**Phase 4 Consideration**: 
- Verify if ASP.NET Core handles this differently (probably not - same issue)
- Not worth fixing (correct behavior is "don't use back button after AJAX mutations")
- Document here so future debugging doesn't waste time

**Similar Issues to Watch For**:
- Any AJAX operation + browser back/forward could have stale state
- Order confirmation page → back button → stale cart display

---

## Album Art - Placeholder Strategy

**Observations**: 

- All albums use single `placeholder.gif`
- database and code does not support having separate thumbnail and full size images for each album
- CSS does not support having a full sized image that could be displayed small as a thumbnailed



**Why Document Here**:

- Not a bug - seed data doesn't include real album art URLs

- Database has `AlbumArtUrl` field but all reference same placeholder

  

------

Shopping Cart Page

- Checkout button is enabled even after all items have been removed.  

Address and Payment Page

- It is possible to complete the checkout process with zero items.

Home Page:

- All menu items and links have two  consist a hover effect:
  -  the color of the text changes
  -  the arrow mouse pointer becomes a pointing finger mouse pointer

​	But there is an exception or omission - the featured album name links do not change color on hover.

-  On the one hand this could be considered a defensible ux choice in that the font is smaller for the featured album names and so you could say "then are not the same as menu items", but I think this is a ux flaw.  I think if the demonstrated standard is hover has a predicable color shift, then that should be true of all links. 

Store Browse Page 

- Same hover issue as described for the home page above. 

Store Details Page

- The "Add to cart" button has no color shift on hover and is a shade of gray so, the user is pretty sure its not disabled but, the ux really had an option to make that clear with the color shift and did not.    And interestingly, while it looks like a button, it is not, it is a text anchor in a gray box, so the user must hover on the text (not just the box) to see the pointer hover effect and click. 

 ```
 <p class="button">
     <a length="0" href="/ShoppingCart/AddToCart/1">Add to cart</a>
 </p>
 ```

- the price is presented without a "$". 