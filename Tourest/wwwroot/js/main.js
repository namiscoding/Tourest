"use strict";

// Custom Cursor  js
// custom cursor
if (document.querySelectorAll(".custom-cursor").length) {
  var cursor = document.querySelector(".custom-cursor__cursor");
  var cursorinner = document.querySelector(".custom-cursor__cursor-two");
  var hyperLinks = document.querySelectorAll("a");

  document.addEventListener("mousemove", function (e) {
    var x = e.clientX;
    var y = e.clientY;
    cursor.style.transform = `translate3d(calc(${x}px - 50%), calc(${y}px - 50%), 0)`;
  });

  document.addEventListener("mousemove", function (e) {
    var x = e.clientX;
    var y = e.clientY;
    cursorinner.style.left = x + "px";
    cursorinner.style.top = y + "px";
  });

  document.addEventListener("mousedown", function () {
    cursor.classList.add("click");
    cursorinner.classList.add("custom-cursor__innerhover");
  });

  document.addEventListener("mouseup", function () {
    cursor.classList.remove("click");
    cursorinner.classList.remove("custom-cursor__innerhover");
  });

  hyperLinks.forEach((item) => {
    item.addEventListener("mouseover", () => {
      cursor.classList.add("custom-cursor__hover");
    });
    item.addEventListener("mouseleave", () => {
      cursor.classList.remove("custom-cursor__hover");
    });
  });
}

// preloader js
function preloader() {
  $("#preloader").delay(800).fadeOut("slow");
}

$(window).on("load", function () {
  preloader();
});

// Fixed menu js
$(window).on("scroll", function () {
  var scroll = $(window).scrollTop();
  if (scroll < 120) {
    $(".header__main").removeClass("sticky-menu");
  } else {
    $(".header__main").addClass("sticky-menu");
  }
});

// meanMenu JS
jQuery(document).ready(function () {
  jQuery("header nav").meanmenu({
    meanScreenWidth: 768,
    meanMenuContainer: ".mobile-menu",
    // meanShowChildren: false,
  });
});

// Nice Select JS
$(document).ready(function () {
  $("select").niceSelect();
});

// Daterangepicker JS
$(function () {
  $('input[name="daterange"]').daterangepicker(
    {
      singleDatePicker: true,
      showDropdowns: true,
      minYear: 1998,
      maxYear: 2030,
      locale: {
        format: "YYYY-MM-DD", // Set the date format to YYYY-MM-DD
      },
    },
    function (start, end, label) {
      console.log(
        "A new date selection was made: " +
          start.format("YYYY-MM-DD") +
          " to " +
          end.format("YYYY-MM-DD")
      );
    }
  );
});
$(function () {
  $('input[name="daterange2"]').daterangepicker(
    {
      singleDatePicker: true,
      showDropdowns: true,
      minYear: 1998,
      maxYear: 2030,
      locale: {
        format: "YYYY-MM-DD",
      },
    },
    function (start, end, label) {
      console.log(
        "A new date selection was made: " +
          start.format("YYYY-MM-DD") +
          " to " +
          end.format("YYYY-MM-DD")
      );
    }
  );
});

function initializeDateRangePicker(selector) {
  var today = moment();

  $(selector).daterangepicker(
    {
      opens: "left",
      startDate: today.startOf("day"),
      endDate: today.startOf("day").add(7, "days"),
      locale: {
        format: "YYYY-MM-DD",
      },
    },
    function (start, end, label) {
      console.log(
        "A new date selection was made: " +
          start.format("YYYY-MM-DD") +
          " to " +
          end.format("YYYY-MM-DD")
      );

      $(selector).addClass("active");
    }
  );

  $(selector).on("cancel.daterangepicker", function () {
    $(this).removeClass("active");
  });
}
// Initialize the date range picker for your input
initializeDateRangePicker('input[name="dateRangePlan"]');

// Destination Slider
const destinationSlider = document.querySelector(".destination__slider");
const destinationPagination = document.querySelector(
  ".destination__pagination"
);
const destinationNext = document.querySelector(".destination__button-next");
const destinationPrev = document.querySelector(".destination__button-prev");

if (destinationSlider && destinationNext && destinationPrev) {
  const swiper = new Swiper(destinationSlider, {
    loop: false,
    spaceBetween: 20,
    parallax: true,

    pagination: {
      el: destinationPagination,
      clickable: true,
    },

    navigation: {
      nextEl: destinationNext,
      prevEl: destinationPrev,
    },

    breakpoints: {
      575: {
        slidesPerView: 2,
        spaceBetween: 20,
      },
      768: {
        slidesPerView: 3,
        spaceBetween: 20,
      },
      991: {
        slidesPerView: 3,
        spaceBetween: 20,
      },
      1199: {
        slidesPerView: 4,
        spaceBetween: 30,
      },
    },
  });
}

// Featured Tours Slider
const featuredToursSlider = document.querySelector(".featured-tours__slider");
const featuredToursPagination = document.querySelector(
  ".featured-tours__pagination"
);

const featuredToursNext = document.querySelector(
  ".featured-tours__button-next"
);

const featuredToursPrev = document.querySelector(
  ".featured-tours__button-prev"
);

if (featuredToursSlider && featuredToursNext && featuredToursPrev) {
  const swiper = new Swiper(featuredToursSlider, {
    loop: true,
    spaceBetween: 20,

    // Pagination settings
    pagination: {
      el: featuredToursPagination,
      clickable: true,
    },

    // Navigation settings
    navigation: {
      nextEl: featuredToursNext,
      prevEl: featuredToursPrev,
    },

    // Responsive breakpoints
    breakpoints: {
      575: {
        slidesPerView: 2,
        spaceBetween: 20,
      },
      991: {
        slidesPerView: 3,
        spaceBetween: 20,
      },
      1023: {
        slidesPerView: 3,
        spaceBetween: 20,
      },
      1199: {
        slidesPerView: 4,
        spaceBetween: 30,
      },
    },
  });
}

// Tours Slider
const activitiesToursSlider = document.querySelector(
  ".activities-tours__slider"
);
const activitiesToursPagination = document.querySelector(
  ".activities-tours__pagination"
);
const activitiesToursNext = document.querySelector(
  ".activities-tours__button-next"
);
const activitiesToursPrev = document.querySelector(
  ".activities-tours__button-prev"
);
if (activitiesToursSlider && activitiesToursNext && activitiesToursPrev) {
  const swiper = new Swiper(activitiesToursSlider, {
    loop: true,
    spaceBetween: 20,

    // Pagination settings
    pagination: {
      el: activitiesToursPagination,
      clickable: true,
    },

    // Navigation settings
    navigation: {
      nextEl: activitiesToursNext,
      prevEl: activitiesToursPrev,
    },

    // Responsive breakpoints
    breakpoints: {
      575: {
        slidesPerView: 2,
        spaceBetween: 20,
      },
      991: {
        slidesPerView: 3,
        spaceBetween: 20,
      },
      1023: {
        slidesPerView: 3,
        spaceBetween: 20,
      },
      1199: {
        slidesPerView: 4,
        spaceBetween: 30,
      },
    },
  });
}

// Testimonia Slider
const testimonialSlider = document.querySelector(".testimonial__slider");
const testimonialPagination = document.querySelector(
  ".testimonial__pagination"
);

if (testimonialSlider && testimonialPagination) {
  const swiper = new Swiper(testimonialSlider, {
    loop: true,
    slidesPerView: 1,
    spaceBetween: 20,
    // effect: 'fade',
    parallax: true,

    // Pagination settings
    pagination: {
      el: testimonialPagination,
      clickable: true,
    },
  });
}

/* Partners Slider */
const partnersSlider = document.querySelector(".partners__slider");
if (partnersSlider) {
  const swiper = new Swiper(partnersSlider, {
    spaceBetween: 30,
    centeredSlides: false,
    speed: 5000,
    autoplay: {
      delay: 1,
    },
    loop: true,
    slidesPerView: "auto",
    allowTouchMove: false,
    disableOnInteraction: true,
    breakpoints: {
      320: {
        spaceBetween: 40,
      },
      992: {
        spaceBetween: 60,
      },
      1200: {
        spaceBetween: 80,
      },
    },
  });
}

/* Tour-selection Slider */
const tourSelectionSlider = document.querySelector(".tour-selection__slider");
const tourSelectionPagination = document.querySelector(
  ".tour-selection__pagination"
);
if (tourSelectionSlider && tourSelectionPagination) {
  const swiper = new Swiper(tourSelectionSlider, {
    loop: true,
    slidesPerView: 2,
    spaceBetween: 20,
    loop: true,
    allowTouchMove: true,
    disableOnInteraction: true,
    pagination: {
      el: tourSelectionPagination,
      clickable: true,
    },
    breakpoints: {
      576: {
        slidesPerView: 3,
      },
      768: {
        slidesPerView: 4,
      },
      992: {
        slidesPerView: 5,
      },
      1200: {
        spaceBetween: 30,
        slidesPerView: 6,
      },
    },
  });
}

/* Team Slider */
const teamSlider = document.querySelector(".team__slider");
const teamPagination = document.querySelector(".team__pagination");
if (teamSlider && teamPagination) {
  const swiper = new Swiper(teamSlider, {
    loop: true,
    slidesPerView: 1,
    spaceBetween: 20,
    loop: true,
    allowTouchMove: true,
    disableOnInteraction: true,
    pagination: {
      el: teamPagination,
      clickable: true,
    },
    breakpoints: {
      576: {
        slidesPerView: 2,
      },
      768: {
        slidesPerView: 3,
      },
      992: {
        slidesPerView: 4,
      },
      1200: {
        spaceBetween: 32,
        slidesPerView: 5,
      },
    },
  });
}

/* Destination-dtls Slider */
const destinationDtlsSlider = document.querySelector(
  ".destination-dtls__slider"
);
const destinationDtlsThumbsSlider = document.querySelector(
  ".destination-dtls__thumbsSlider"
);

if (destinationDtlsSlider && destinationDtlsThumbsSlider) {
  var swiper = new Swiper(destinationDtlsThumbsSlider, {
    loop: true,
    spaceBetween: 10,
    slidesPerView: 3,
    freeMode: true,
    watchSlidesProgress: true,

    breakpoints: {
      575: {
        spaceBetween: 20,
      },
      768: {},
      991: {
        slidesPerView: 4,
      },
      1199: {
        slidesPerView: 5,
        spaceBetween: 20,
      },
    },
  });
  var swiper2 = new Swiper(destinationDtlsSlider, {
    loop: true,
    spaceBetween: 10,
    thumbs: {
      swiper: swiper,
    },
  });
}

// Odometer Counter
$(".counter-item").each(function () {
  var $counterItem = $(this); // Store reference to the specific counter item
  $counterItem.isInViewport(function (status) {
    if (status === "entered") {
      // Find the odometer within the specific counter item
      $counterItem.find(".odometer").each(function () {
        var el = this;
        el.innerHTML = el.getAttribute("data-odometer-final");
      });
    }
  });
});

// Magnific-Popup Counter Start
$(document).ready(function () {
  $(".popup-youtube, .popup-vimeo, .popup-gmaps").magnificPopup({
    type: "iframe",
    mainClass: "mfp-fade",
    removalDelay: 160,
    preloader: false,

    fixedContentPos: false,
  });
});

$(document).ready(function () {
  $(".popup-gallery").magnificPopup({
    delegate: "a",
    type: "image",
    tLoading: "Loading image #%curr%...",
    mainClass: "mfp-img-mobile",
    gallery: {
      enabled: true,
      navigateByImgClick: true,
      preload: [0, 1],
    },
    image: {
      tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
    },
  });
});

/* Price Filter */
$(function () {
  // Check if required elements exist
  if (
    $("#slider-range").length &&
    $("#amount").length &&
    $("#amount2").length
  ) {
    // Initialize the slider
    $("#slider-range").slider({
      range: true,
      min: 0,
      max: 15000,
      values: [2000, 13000],
      slide: function (event, ui) {
        // Update input fields during slider movement
        $("#amount").val(ui.values[0] + " $");
        $("#amount2").val(ui.values[1] + " $");
      },
    });

    // Set initial values for the input fields
    $("#amount").val($("#slider-range").slider("values", 0) + " $");
    $("#amount2").val($("#slider-range").slider("values", 1) + " $");

    // Add event listeners to input fields to update slider
    $("#amount, #amount2").on("change", function () {
      // Get numeric values from input fields
      const value1 = parseInt($("#amount").val().replace(/\D/g, ""), 10);
      const value2 = parseInt($("#amount2").val().replace(/\D/g, ""), 10);

      // Validate and update slider values
      if (!isNaN(value1) && !isNaN(value2) && value1 <= value2) {
        $("#slider-range").slider("values", [value1, value2]);
      } else {
        // Reset to valid values if input is invalid
        $("#amount").val($("#slider-range").slider("values", 0) + " $");
        $("#amount2").val($("#slider-range").slider("values", 1) + " $");
      }
    });
  }
});

const dateInput = document.getElementById("dateRangePlan");
const timeSelect = document.getElementById("time");
const adultsInput = document.getElementById("adults");
const kidsInput = document.getElementById("kids");
const childrenInput = document.getElementById("children");
const totalDisplay = document.getElementById("total");
const bookNowButton = document.getElementById("book-now");

if (
  dateInput &&
  timeSelect &&
  adultsInput &&
  kidsInput &&
  childrenInput &&
  totalDisplay &&
  bookNowButton
) {
  adultsInput.value = 1;

  function populateTimeOptions() {
    for (let i = 8; i <= 22; i++) {
      const option = document.createElement("option");
      option.value = `${i}:00`;
      option.textContent = `${i}:00`;
      timeSelect.appendChild(option);
    }
  }

  // Update total calculation with more robust pricing
  function updateTotal() {
    const adults = parseInt(adultsInput.value) || 0;
    const kids = parseInt(kidsInput.value) || 0;
    const children = parseInt(childrenInput.value) || 0;

    // Different pricing tiers
    const adultPrice = 104;
    const kidPrice = 84;
    const childPrice = 52;

    const total = adults * adultPrice + kids * kidPrice + children * childPrice;

    totalDisplay.textContent = `$${total}`;

    // Optional: Enable/disable book now button based on total
    bookNowButton.disabled = total === 0;
  }

  // Create reusable increment/decrement function
  function createIncrementDecrementHandlers(inputElement) {
    const decrementBtn = document.getElementById(
      `${inputElement.id}-decrement`
    );
    const incrementBtn = document.getElementById(
      `${inputElement.id}-increment`
    );

    decrementBtn.addEventListener("click", () => {
      inputElement.value = Math.max(0, parseInt(inputElement.value) - 1);
      updateTotal();
    });

    incrementBtn.addEventListener("click", () => {
      inputElement.value = parseInt(inputElement.value) + 1;
      updateTotal();
    });
  }

  // Setup event listeners
  function initializeBookingForm() {
    populateTimeOptions();

    // Initial total calculation
    updateTotal();

    // Add input event listeners
    [adultsInput, kidsInput, childrenInput].forEach((input) => {
      input.addEventListener("input", updateTotal);
    });

    // Setup increment/decrement for each input
    createIncrementDecrementHandlers(adultsInput);
    createIncrementDecrementHandlers(kidsInput);
    createIncrementDecrementHandlers(childrenInput);
  }

  // Initialize the form when the script loads
  initializeBookingForm();
}

// filter Short By
const filterShortBy = document.querySelector(".filter-short-by");

if (filterShortBy) {
  filterShortBy.addEventListener("click", function () {
    const downIcon = filterShortBy.querySelector(".fa-arrow-down-a-z");
    const upIcon = filterShortBy.querySelector(".fa-arrow-up-a-z");

    if (downIcon.classList.contains("active")) {
      downIcon.classList.remove("active");
      upIcon.classList.add("active");
    } else {
      upIcon.classList.remove("active");
      downIcon.classList.add("active");
    }
  });
}

// progressBars animation using GSAP
gsap.registerPlugin(ScrollTrigger);

const progressBars = document.querySelectorAll(".progress-bar");

if (progressBars.length > 0) {
  progressBars.forEach((bar) => {
    const targetWidth = bar.style.width;

    bar.style.width = "0%";

    gsap.to(bar, {
      width: targetWidth,
      duration: 0.5,
      ease: "none",
      scrollTrigger: {
        trigger: bar,
        start: "top 80%",
        toggleActions: "play none none none",
        markers: false,
      },
    });
  });
}

if ($(".char-animation").length > 0) {
  let char_come = gsap.utils.toArray(".char-animation");
  char_come.forEach((splitTextLine) => {
    const tl = gsap.timeline({
      scrollTrigger: {
        trigger: splitTextLine,
        start: "top bottom",
        end: "bottom top",
        scrub: false,
        markers: false,
        toggleActions: "play none none none",
      },
    });

    const itemSplitted = new SplitText(splitTextLine, { type: "chars, words" });
    gsap.set(splitTextLine, { perspective: 300 });
    itemSplitted.split({ type: "chars, words" });
    tl.from(itemSplitted.chars, {
      duration: 1,
      delay: 0.5,
      x: 100,
      autoAlpha: 0,
      stagger: 0.05,
    });
  });
}


  // Title Anim

  let splitTitleLines = gsap.utils.toArray(".title-anim");

  splitTitleLines.forEach(splitTextLine => {
    const tl = gsap.timeline({
      scrollTrigger: {
        trigger: splitTextLine,
        start: 'top 90%',
        end: 'bottom 60%',
        scrub: false,
        markers: false,
        toggleActions: 'play none none none'
      }
    });

    const itemSplitted = new SplitText(splitTextLine, { type: "words, lines" });
    gsap.set(splitTextLine, { perspective: 400 });
    itemSplitted.split({ type: "lines" })
    tl.from(itemSplitted.lines, { duration: 1, delay: 0.3, opacity: 0, rotationX: -80, force3D: true, transformOrigin: "top center -50", stagger: 0.1 });
  });

  // Title Animation End

// Fade Animation

if ($(".fade_top").length > 0) {
  gsap.set(".fade_top", { y: -100, opacity: 0 });
  const fadeArray = gsap.utils.toArray(".fade_top");
  fadeArray.forEach((item, i) => {
    let fadeTl = gsap.timeline({
      scrollTrigger: {
        trigger: item,
        start: "top center+=400",
      },
    });
    fadeTl.to(item, {
      y: 0,
      opacity: 1,
      ease: "power2.out",
      duration: 1.5,
      stagger: {
        each: 0.2,
      },
    });
  });
}
if ($(".fade_bottom").length > 0) {
  gsap.set(".fade_bottom", { y: 100, opacity: 0 });
  const fadeArray = gsap.utils.toArray(".fade_bottom");
  fadeArray.forEach((item, i) => {
    let fadeTl = gsap.timeline({
      scrollTrigger: {
        trigger: item,
        start: "top center+=400",
      },
    });
    fadeTl.to(item, {
      y: 0,
      opacity: 1,
      ease: "power2.out",
      duration: 1.5,
      stagger: {
        each: 0.2,
      },
    });
  });
}

if ($(".fade_left").length > 0) {
  gsap.set(".fade_left", { x: -100, opacity: 0 });
  const fadeleftArray = gsap.utils.toArray(".fade_left");
  fadeleftArray.forEach((item, i) => {
    let fadeTl = gsap.timeline({
      scrollTrigger: {
        trigger: item,
        start: "top center+=100",
      },
    });
    fadeTl.to(item, {
      x: 0,
      opacity: 1,
      ease: "power2.out",
      duration: 2.5,
    });
  });
}

if ($(".fade_right").length > 0) {
  gsap.set(".fade_right", { x: 100, opacity: 0 });
  const faderightArray = gsap.utils.toArray(".fade_right");
  faderightArray.forEach((item, i) => {
    let fadeTl = gsap.timeline({
      scrollTrigger: {
        trigger: item,
        start: "top center+=100",
      },
    });
    fadeTl.to(item, {
      x: 0,
      opacity: 1,
      ease: "power2.out",
      duration: 2.5,
    });
  });
}

// Fade Animation End

  // Button bounce

  if ($('.btn-trigger').length > 0) {

		gsap.set(".btn-bounce", { y: -100, opacity: 0 });
		var mybtn = gsap.utils.toArray(".btn-bounce");
		mybtn.forEach((btn) => {
			var $this = $(btn);
			gsap.to(btn, {
				scrollTrigger: {
					trigger: $this.closest('.btn-trigger'),
					start: "top center",
					markers: false
				},
				duration: 1,
				ease: "bounce.out",
				y: 0,
				opacity: 1,
			})
		});

	} // Button bounce End


/* ----------- item-stagger Animation Start ----------- */
function initBrandAnimations() {
  const brandItems = document.querySelectorAll(".item-stagger");

  if (!brandItems || brandItems.length === 0) {
    return;
  }

  function getItemsByRow() {
    const items = Array.from(brandItems);
    const rows = {};

    items.forEach((item) => {
      const offsetTop = item.offsetTop;
      if (!rows[offsetTop]) {
        rows[offsetTop] = [];
      }
      rows[offsetTop].push(item);
    });

    return Object.values(rows);
  }

  const itemRows = getItemsByRow();

  if (!itemRows || itemRows.length === 0) {
    console.log("Error grouping items into rows");
    return;
  }

  try {
    itemRows.forEach((rowItems) => {
      if (!rowItems || rowItems.length === 0) return;

      gsap.set(rowItems, {
        opacity: 0,
        y: 50,
      });

      gsap.to(rowItems, {
        scrollTrigger: {
          trigger: rowItems[0],
          start: "top bottom",
          end: "bottom center",
          toggleActions: "play none none none",
          markers: false,
        },
        opacity: 1,
        y: 0,
        duration: 0.6,
        stagger: {
          amount: 1,
          from: "start",
        },
        ease: "power2.out",
      });
    });
  } catch (error) {
    console.error("Error initializing brand animations:", error);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  initBrandAnimations();
});

// Bottom to top start
$(document).ready(function () {
  $(window).on("scroll", function () {
    if ($(this).scrollTop() > 100) {
      $("#scroll-top").fadeIn();
    } else {
      $("#scroll-top").fadeOut();
    }
  });
  $("#scroll-top").on("click", function () {
    $("html, body").animate({ scrollTop: 0 }, 600);
    return false;
  });
});
// Bottom to top End
