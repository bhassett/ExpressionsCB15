jQuery(function (jQuery) {

    'use strict';
    var jQuery = jQuery.noConflict();
    /**
     * ==============================
     * Page Loader
     * ==============================
     */
    function addCSS(css) {

        var head = document.getElementsByTagName('head')[0],
                stylez = document.createElement('style');
        stylez.setAttribute('type', 'text/css');
        if (stylez.styleSheet) {
            stylez.styleSheet.cssText = css;
        } else {
            stylez.appendChild(document.createTextNode(css));
        }

        head.appendChild(stylez);
    }

    addCSS('<style>#full-site-wrapper { display: none; opacity: 0; }</style>')

    jQuery(window).load(function () {

        setTimeout(function () {
            jQuery(".preloader-wrap").fadeOut(300);
            jQuery("#full-site-wrapper").show().animate({
                opacity: 1
            }, 150);
        }, 3100);

        /**
         * ==============================
         * ISOTOPE
         * ==============================
         */

        if (jQuery().isotope) {
            var jQuerycontainer = jQuery('.isotope'); // cache container
            jQuerycontainer.isotope({
                itemSelector: '.isotope-item'
            });
            jQuery('.filtrable a').click(function () {
                var selector = jQuery(this).attr('data-filter');
                jQuery('.filtrable li').removeClass('current');
                jQuery(this).parent().addClass('current');
                jQuerycontainer.isotope({filter: selector});
                return false;
            });
            jQuerycontainer.isotope('layout'); // layout/layout

        }

    });
    jQuery(window).resize(function () {
        if (jQuery().isotope) {
            jQuery('.row.isotope').isotope('layout'); // layout/relayout on window resize
        }
    });
    jQuery('#product-filter').isotope({filter: '.tab-1'});
    // ---------------------------------------------------------------------------------------

    /**
     * ==============================
     *  Scroll To Top 
     * ==============================
     */

    jQuery(window).scroll(function () {
        if (jQuery(this).scrollTop() > 100) {
            jQuery('.to-top').css({bottom: '55px'});
        }
        else {
            jQuery('.to-top').css({bottom: '-150px'});
        }
    });
    jQuery(function ($) {
        /**
         * ==============================
         *  Remove Active Class 
         * ==============================
         */

        jQuery(document).click(function (e) {
            var active = e.target ? jQuery(e.target).closest('.active').get(0) : null;
            jQuery(".header-wrap .navigation").removeClass('off-canvas');
            jQuery(".hdr-stl-2").removeClass('off-canvas');
            jQuery("body").removeClass('off-canvas-body');
            jQuery(".top-elements li" + ",.nav-trigger").filter(function () {
                return !active || active !== this;
            }).removeClass('active');
        });

        /**
         * ==============================
         *  Header Off Canvas Add 
         * ==============================
         */

        jQuery(".nav-trigger").on("click", function (e) {
            e.stopPropagation();
            jQuery(".header-wrap .navigation").toggleClass("off-canvas");
            jQuery(".hdr-stl-2").toggleClass("off-canvas");
            if ($(".hdr-stl-2").length) {
                jQuery("body").toggleClass("off-canvas-body");
            }
        });

        /**
         * ==============================
         *  Scroll To Top Animate
         * ==============================
         */

        jQuery('.to-top').click(function () {
            jQuery('html, body').animate({scrollTop: 0}, 800);
            return false;
        });
        /**
         * ==============================
         *  Header PopUps
         * ==============================
         */

        jQuery(".toggle-hover").each(function () {
            var $toggle = jQuery(this);
            $toggle.children('a').click(function (e) {
                e.preventDefault();
                $toggle.toggleClass("active");
            });
        });

        /**
         * ==============================
         *  Sticky Header
         * ==============================
         */

        if (jQuery(window).width() > 760 && (jQuery('.main-header').length > 0)) {
            jQuery(".main-header").sticky({topSpacing: 0});
        }

        if (jQuery(window).width() < 767) {
            if (jQuery(".top-bar .navigation").length) {
                jQuery(".top-bar .navigation").mCustomScrollbar({
                    theme: "dark-2",
                    scrollButtons: {
                        enable: false
                    }
                });
            }
        }


        /**
         * ==============================
         * Scroll in sidebar widget
         * ==============================
         */
        if (jQuery(".widget-wrap .scroll").length) {
            jQuery(".widget-wrap .scroll").mCustomScrollbar({
                theme: "dark-2",
                scrollButtons: {
                    enable: false
                }
            });
        }


        /**
         * ==============================
         * Product Slider
         * ==============================
         */
        if (jQuery('.product-slider').length > 0) {
            jQuery(".product-slider").owlCarousel({
                dots: false,
                loop: true,
                nav: true,
                autoplay: true,
                autoplayHoverPause: true,
                smartSpeed: 100,
                margin: 30,
                responsive: {
                    0: {items: 1},
                    1200: {items: 5},
                    992: {items: 5},
                    768: {items: 3},
                    600: {items: 2},
                    480: {items: 2}
                },
                navText: [
                    "<i class='fa fa-chevron-left'></i>",
                    "<i class='fa fa-chevron-right'></i>"
                ]
            });
        }

        /**
         * ==============================
         * Product Image Slider
         * ==============================
         */
        if (jQuery('.cat-slider').length > 0) {
            jQuery(".cat-slider").owlCarousel({
                dots: false,
                loop: true,
                items: 1,
                nav: true,
                autoplay: true,
                autoplayHoverPause: true,
                smartSpeed: 100,
                margin: 30,
                responsive: {
                    0: {items: 1}
                },
                navText: [
                    "<i class='fa fa-chevron-left'></i>",
                    "<i class='fa fa-chevron-right'></i>"
                ]
            });
        }


        /**
         * ==============================
         * Product Timeline Slider
         * ==============================
         */
        if (jQuery('.timeline-slider').length > 0) {
            jQuery(".timeline-slider").owlCarousel({
                items: 4,
                lazyLoad: true,
                autoplay: false,
                stopOnHover: true,
                transitionStyle: "fade",
                touchDrag: false,
                mouseDrag: false,
                loop: true,
                dots: true,
                nav: false,
                responsive: {
                    0: {items: 1},
                    768: {items: 2}
                }
            });
        }

        /**
         * ==============================
         * Product Tooltip
         * ==============================
         */
        if (jQuery('.tooltip-demo').length > 0) {
            jQuery('.tooltip-demo').tooltipster({
                contentAsHTML: true,
                position: 'right'
            });
        }

        /**
         * ==============================
         *  Recent Product Slider
         * ==============================
         */

        if (jQuery('.recent-prod-slider').length > 0) {
            jQuery(".recent-prod-slider").owlCarousel({
                dots: true,
                loop: true,
                autoplay: true,
                autoplayHoverPause: true,
                smartSpeed: 100,
                responsive: {
                    0: {items: 1},
                    1200: {items: 1},
                    767: {items: 1},
                    600: {items: 2},
                    480: {items: 1}
                }
            });
        }

        /**
         * ==============================
         *  Price Range
         * ==============================
         */

        var priceSliderRange = $('.slider-range');
        if ($.ui) {
            if ($(priceSliderRange).length) {
                $(priceSliderRange).slider({
                    range: true,
                    min: 0,
                    max: 500,
                    values: [16, 200],
                    slide: function (event, ui) {
                        $("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);
                    }
                });
                $("#amount").val(
                        "$" + $(".slider-range").slider("values", 0) +
                        " - $" + $(".slider-range").slider("values", 1)
                        );
            }
        }
    });

    /**
     * ==========================================
     * Product Sidebar-3 Accordion Icon Change
     * ==========================================
     */
    function toggleChevron(e) {
        jQuery(e.target)
                .prev('.panel-heading')
                .find("i.indicator")
                .toggleClass('icon_minus-box icon_plus-box');
    }
    jQuery('#accordion').on('hidden.bs.collapse', toggleChevron);
    jQuery('#accordion').on('shown.bs.collapse', toggleChevron);


    /**
     * ==============================
     * Product Slider With Tabs  
     * ==============================
     */

    var swiperslider1 = jQuery('.swiper-slider-1 .swiper-container');
    var swiperslider2 = jQuery('.swiper-slider-2 .swiper-container');
    var swiperslider3 = jQuery('.swiper-slider-3 .swiper-container');
    jQuery('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        updater();
    });
    if (jQuery().swiper) {
        //Product Slider 1
        if (swiperslider1.length) {
            swiperslider1 = new Swiper(swiperslider1, {
                pagination: '.swiper-pagination',
                slidesPerView: 4,
                paginationClickable: true,
                spaceBetween: 30,
                loop: true,
                nextButton: '.swiper-slider-1 .slide-next',
                prevButton: '.swiper-slider-1 .slide-prev',
                breakpoints: {
                    481: {
                        slidesPerView: 1,
                        spaceBetweenSlides: 10
                    },
                    991: {
                        slidesPerView: 2,
                        spaceBetweenSlides: 20
                    },
                    1199: {
                        slidesPerView: 3,
                        spaceBetweenSlides: 30
                    }
                }
            });
        }

        //Product Slider 2
        if (swiperslider2.length) {
            swiperslider2 = new Swiper(swiperslider2, {
                slidesPerView: 4,
                paginationClickable: true,
                spaceBetween: 30,
                loop: true,
                nextButton: '.swiper-slider-2 .slide-next',
                prevButton: '.swiper-slider-2 .slide-prev',
                breakpoints: {
                    481: {
                        slidesPerView: 1,
                        spaceBetweenSlides: 10
                    },
                    991: {
                        slidesPerView: 2,
                        spaceBetweenSlides: 20
                    },
                    1199: {
                        slidesPerView: 3,
                        spaceBetweenSlides: 30
                    }
                }
            });
        }

        //Product Slider 3
        if (swiperslider3.length) {
            swiperslider3 = new Swiper(swiperslider3, {
                slidesPerView: 4,
                paginationClickable: true,
                spaceBetween: 30,
                loop: true,
                nextButton: '.swiper-slider-3 .slide-next',
                prevButton: '.swiper-slider-3 .slide-prev',
                breakpoints: {
                    481: {
                        slidesPerView: 1,
                        spaceBetweenSlides: 10
                    },
                    991: {
                        slidesPerView: 2,
                        spaceBetweenSlides: 20
                    },
                    1199: {
                        slidesPerView: 3,
                        spaceBetweenSlides: 30
                    }
                }
            });
        }
    }
    updater();
    /**
     * ==============================
     * swiper slider 
     * ==============================
     */

    function updater() {
        // refresh swiper slider
        if (jQuery().swiper) {
            //
            if (typeof (swiperslider1.length) === 'undefined') {
                swiperslider1.update();
                swiperslider1.onResize();
            }
            //
            if (typeof (swiperslider2.length) === 'undefined') {
                swiperslider2.update();
                swiperslider2.onResize();
            }
            //
            if (typeof (swiperslider3.length) === 'undefined') {
                swiperslider3.update();
                swiperslider3.onResize();
            }
        }
    }


});
//Slider Product
$(document).ready(function ($) {

    /**
     * ==============================
     * Product Preview Popup Slider
     * ==============================
     */
    $(document).on('shown.bs.modal', function () {
        $(this).find('.sync1, .sync2').each(function () {
            $(this).data('owlCarousel') ? $(this).data('owlCarousel').onResize() : null;
        });
    });
    var sync1 = $(".sync1");
    var sync2 = $(".sync2");
    var navSpeedThumbs = 500;
    sync2.owlCarousel({
        rtl: false,
        items: 4,
        navSpeed: navSpeedThumbs,
        responsive: {
            768: {items: 4},
            480: {items: 4},
            0: {items: 4}
        },
        responsiveRefreshRate: 200
    });
    sync1.owlCarousel({
        rtl: false,
        items: 1,
        navSpeed: 1000,
        nav: true,
        onChanged: syncPosition,
        responsiveRefreshRate: 200,
        navText: [
            "<i class='fa fa-chevron-left'></i>",
            "<i class='fa fa-chevron-right'></i>"
        ]
    });
    function syncPosition(el) {
        var current = this._current;
        $(".sync2")
                .find(".owl-item")
                .removeClass("synced")
                .eq(current)
                .addClass("synced");
        center(current);
    }

    $(".sync2").on("click", ".owl-item", function (e) {
        e.preventDefault();
        var number = $(this).index();
        sync1.trigger("to.owl.carousel", [number, 1000]);
    });
    function center(num) {

        var sync2visible = sync2.find('.owl-item.active').map(function () {
            return $(this).index();
        });
        if ($.inArray(num, sync2visible) === -1) {
            if (num > sync2visible[sync2visible.length - 1]) {
                sync2.trigger("to.owl.carousel", [num - sync2visible.length + 2, navSpeedThumbs, true]);
            } else {
                sync2.trigger("to.owl.carousel", Math.max(0, num - 1));
            }
        } else if (num === sync2visible[sync2visible.length - 1]) {
            sync2.trigger("to.owl.carousel", [sync2visible[1], navSpeedThumbs, true]);
        } else if (num === sync2visible[0]) {
            sync2.trigger("to.owl.carousel", [Math.max(0, num - 1), navSpeedThumbs, true]);
        }
    }

    /**
     * ==============================
     * Product Prettyphoto
     * ==============================
     */
    if ($.fn.prettyPhoto) {
        $("a[data-gal^='prettyPhoto']").prettyPhoto({
            theme: 'facebook',
            slideshow: 5000,
            autoplay_slideshow: true
        });
    }

    /**
     * ==============================
     * Product Masonery
     * ==============================
     */
    if (jQuery('.product-masonry').length) {
        $('.product-masonry').isotope({
            itemSelector: '.product-masonry .cate-box',
            percentPosition: true,
            masonry: {
                columnWidth: '.grid-sizer'
            }
        });
    }
});